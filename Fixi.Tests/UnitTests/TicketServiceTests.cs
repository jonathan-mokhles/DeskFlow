using AutoFixture;
using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.Domain.IdentityEntity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.DTOs.shared;
using DeskFkow.Core.DTOs.TicketDTOs;
using DeskFkow.Core.Enums;
using DeskFkow.Core.Exceptions;
using DeskFkow.Core.Services;
using DeskFkow.Core.ServicesContracts;
using DeskFkow.Core.ServicesContracts.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace DeskFkow.Tests.UnitTests
{
    public class TicketServiceTests
    {
        Mock<IUnitOfWork> _unitOfWork;
        Mock<IIdentityService> _identityService;
        Mock<IBackgroundJobService> _backgroundJob;
        Mock<ICurrentUserService> _currentUserService;
        ITicketService _ticketService;
        Fixture _fixture;


        public TicketServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _identityService = new Mock<IIdentityService>();
            _backgroundJob = new Mock<IBackgroundJobService>();
            _currentUserService = new Mock<ICurrentUserService>();

            _unitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);

            _ticketService = new TicketService(_unitOfWork.Object, _identityService.Object, _backgroundJob.Object, _currentUserService.Object);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        #region CreateTicket

        [Fact]
        public async Task CreteTicket_WithMissingSLA_ThrowValidationException()
        {
            CreateTicketDTO ticketDTO = _fixture.Create<CreateTicketDTO>();
            _slaRepo.Setup(x =>  x.GetByPriorityAsync(ticketDTO.Priority)).ReturnsAsync((SLASetting)null);


            var act = async  () => await _ticketService.CreateTicketAsync(ticketDTO, "userId");
             await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreteTicket_WithValidData_AddTicket()
        {
            var ticketDTO = _fixture.Create<CreateTicketDTO>();
            var SLA = _fixture.Create<SLASetting>();
            Ticket ticket = new Ticket
            {
                Title = ticketDTO.Title,
                Description = ticketDTO.Description,
                Priority = (TicketPriority)ticketDTO.Priority,
                Status = TicketStatus.Open,
                CategoryId = ticketDTO.CategoryId,
                ReportedById = "userId",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                SLAResolutionDeadline = DateTime.UtcNow.AddMinutes(SLA.ResolutionTimeMinutes),
                SLAResponseDeadline = DateTime.UtcNow.AddMinutes(SLA.ResponseTimeMinutes),
                LastModifiedById = "userId"

            };
            TicketAuditLog audit = new TicketAuditLog
            {
                TicketId = ticket.Id,
                ChangedById = "userId",
                ChangeType = "Created",
                ChangedDate = ticket.CreatedDate,
                OldValue = null,
                NewValue = $"Title: {ticket.Title}, Description: {ticket.Description}, Priority: {ticket.Priority}, Status: {ticket.Status}, CategoryId: {ticket.CategoryId}"
            };


            _slaRepo.Setup(x =>  x.GetByPriorityAsync(It.IsAny<int>())).ReturnsAsync(SLA);
            _ticketRepository.Setup(x => x.CreateAsync(It.IsAny<Ticket>())).ReturnsAsync(ticket);
            _ticketAuditLogRepo.Setup(x => x.CreateAsync(It.IsAny<TicketAuditLog>())).ReturnsAsync(audit);


            var act = async  () => await _ticketService.CreateTicketAsync(ticketDTO, "userId");
             await act.Should().NotThrowAsync();
            _ticketRepository.Verify(x => x.CreateAsync(It.IsAny<Ticket>()),Times.Once);
            _ticketAuditLogRepo.Verify(x => x.CreateAsync(It.IsAny<TicketAuditLog>()),Times.Once);


        }

        #endregion

        #region UpdateTicket

        [Fact]
        public async Task UpdateTicket_WithWrongId_ThrowNotFoundException()
        {
            UpdateTicketDTO UpdateDTO = _fixture.Create<UpdateTicketDTO>();
            _ticketRepository.Setup(x =>  x.GetTicketAsync(UpdateDTO.Id)).ReturnsAsync((TicketDTO)null);

            var act = async  () => await _ticketService.UpdateTicketAsync(UpdateDTO, new UserClaims());
            await act.Should().ThrowAsync<TicketNotFoundException>();
        }
        [Fact]
        public async Task UpdateTicket_NotReporter_ThrowUnauthorizedTicketAccessException()
        {
            UpdateTicketDTO UpdateDTO = _fixture.Create<UpdateTicketDTO>();
            TicketDTO ticketDTO = _fixture.Create<TicketDTO>();
            _ticketRepository.Setup(x =>  x.GetTicketAsync(UpdateDTO.Id)).ReturnsAsync(ticketDTO);

            var act = async  () => await _ticketService.UpdateTicketAsync(UpdateDTO, new UserClaims() { UserId = "userID" });
            await act.Should().ThrowAsync<UnauthorizedTicketAccessException>();
        }
        [Fact]
        public async Task UpdateTicket_ValidData_UpdateTicket()
        {
            UpdateTicketDTO UpdateDTO = _fixture.Create<UpdateTicketDTO>();
            TicketDTO ticketDTO = _fixture.Create<TicketDTO>();

            _ticketRepository.Setup(x =>  x.GetTicketAsync(UpdateDTO.Id)).ReturnsAsync(ticketDTO);
            _ticketAuditLogRepo.Setup(x => x.CreateAsync(It.IsAny<TicketAuditLog>())).ReturnsAsync(new TicketAuditLog());
            _ticketRepository.Setup(x => x.UpdateAsync(It.IsAny<UpdateTicketDTO>())).Returns(Task.CompletedTask);

            var act = async  () => await _ticketService.UpdateTicketAsync(UpdateDTO, new UserClaims() { UserId = ticketDTO.ReportedById });
            await act.Should().NotThrowAsync();
            _ticketAuditLogRepo.Verify(x => x.CreateAsync(It.IsAny<TicketAuditLog>()),Times.Once);
            _ticketRepository.Verify(x => x.UpdateAsync(It.IsAny<UpdateTicketDTO>()),Times.Once);


        }



        #endregion

        #region AssignTechnician

        [Fact]
        public async Task AssignTechnician_WithMissingTicket_ThrowsTicketNotFoundException()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-1";
            var claims = new UserClaims { UserId = "user-1", DeptId = 1, Role = "Manager" };

            _ticketRepository.Setup(x => x.GetTicketAsync(ticketId)).ReturnsAsync((TicketDTO)null);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId, claims);

            await act.Should().ThrowAsync<TicketNotFoundException>();
        }

        [Fact]
        public async Task AssignTechnician_WithDifferentDepartment_ThrowsUnauthorizedTicketAccessException()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-1";
            var claims = new UserClaims { UserId = "user-1", DeptId = 1, Role = "Manager" };
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.DepartmentId, 2)
                .Create();

            _ticketRepository.Setup(x => x.GetTicketAsync(ticketId)).ReturnsAsync(ticket);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId, claims);

            await act.Should().ThrowAsync<UnauthorizedTicketAccessException>();
        }

        [Fact]
        public async Task AssignTechnician_TechnicianAssignsOther_ThrowsBusinessRuleViolationException()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-2";
            var claims = new UserClaims { UserId = "tech-1", DeptId = 1, Role = "Technician" };
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.DepartmentId, 1)
                .Create();

            _ticketRepository.Setup(x => x.GetTicketAsync(ticketId)).ReturnsAsync(ticket);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId, claims);

            await act.Should().ThrowAsync<BusinessRuleViolationException>();
        }

        [Fact]
        public async Task AssignTechnician_WithMissingTechnician_ThrowsNotFoundException()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-1";
            var claims = new UserClaims { UserId = "user-1", DeptId = 1, Role = "Manager" };
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.DepartmentId, 1)
                .Create();

            _ticketRepository.Setup(x => x.GetTicketAsync(ticketId)).ReturnsAsync(ticket);
            _identityService.Setup(x => x.FindByIdAsync(newTechnicianId)).ReturnsAsync((ApplicationUser)null);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId, claims);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task AssignTechnician_WithTechnicianFromDifferentDepartment_ThrowsBusinessRuleViolationException()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-1";
            var claims = new UserClaims { UserId = "user-1", DeptId = 1, Role = "Manager" };
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.DepartmentId, 1)
                .Create();
            var technician = new ApplicationUser { DepartmentId = 2, FullName = "Tech User" };

            _ticketRepository.Setup(x => x.GetTicketAsync(ticketId)).ReturnsAsync(ticket);
            _identityService.Setup(x => x.FindByIdAsync(newTechnicianId)).ReturnsAsync(technician);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId, claims);

            await act.Should().ThrowAsync<BusinessRuleViolationException>();
        }

        [Fact]
        public async Task AssignTechnician_WithValidData_AssignsTechnicianAndCreatesAuditLog()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-1";
            var claims = new UserClaims { UserId = "manager-1", DeptId = 1, Role = "Manager" };
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.DepartmentId, 1)
                .With(t => t.AssignedToFullname, (string?)null)
                .Create();
            var technician = new ApplicationUser { DepartmentId = 1, FullName = "Tech User" };

            _ticketRepository.Setup(x => x.GetTicketAsync(ticketId)).ReturnsAsync(ticket);
            _identityService.Setup(x => x.FindByIdAsync(newTechnicianId)).ReturnsAsync(technician);
            _ticketAuditLogRepo.Setup(x => x.CreateAsync(It.IsAny<TicketAuditLog>())).ReturnsAsync(new TicketAuditLog());
            _ticketRepository.Setup(x => x.AssignTechnician(ticketId, newTechnicianId)).Returns(Task.CompletedTask);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId, claims);

            await act.Should().NotThrowAsync();
            _ticketAuditLogRepo.Verify(x => x.CreateAsync(It.IsAny<TicketAuditLog>()), Times.Once);
            _ticketRepository.Verify(x => x.AssignTechnician(ticketId, newTechnicianId), Times.Once);
        }

        #endregion

        #region GetTicketById

        [Fact]
        public async Task GetTicketByIdAsync_WithValidId_ReturnsTicket()
        {
            var ticketId = 10;
            var expected = _fixture.Create<TicketFullResponseDTO>();

            _ticketRepository.Setup(x => x.GetFullTicketAsync(ticketId)).ReturnsAsync(expected);

            var result = await _ticketService.GetTicketByIdAsync(ticketId);

            result.Should().BeEquivalentTo(expected);
            _ticketRepository.Verify(x => x.GetFullTicketAsync(ticketId), Times.Once);
        }

        #endregion

        #region DeleteTicket

        [Fact]
        public async Task DeleteTicket_WithMissingTicket_ThrowsTicketNotFoundException()
        {
            var ticketId = 5;
            var userId = "user-1";

            _ticketRepository.Setup(x => x.GetTicketAsync(ticketId)).ReturnsAsync((TicketDTO)null);

            var act = async () => await _ticketService.DeleteTicket(ticketId, userId);

            await act.Should().ThrowAsync<TicketNotFoundException>();
        }

        [Fact]
        public async Task DeleteTicket_WithValidTicket_CreatesAuditLogAndDeletes()
        {
            var ticketId = 5;
            var userId = "user-1";
            var ticket = _fixture.Create<TicketDTO>();

            _ticketRepository.Setup(x => x.GetTicketAsync(ticketId)).ReturnsAsync(ticket);
            _ticketAuditLogRepo.Setup(x => x.CreateAsync(It.IsAny<TicketAuditLog>())).ReturnsAsync(new TicketAuditLog());
            _ticketRepository.Setup(x => x.DeleteAsync(ticketId)).Returns(Task.CompletedTask);

            var act = async () => await _ticketService.DeleteTicket(ticketId, userId);

            await act.Should().NotThrowAsync();
            _ticketAuditLogRepo.Verify(x => x.CreateAsync(It.IsAny<TicketAuditLog>()), Times.Once);
            _ticketRepository.Verify(x => x.DeleteAsync(ticketId), Times.Once);
        }

        #endregion

        #region GetAllTickets

        [Fact]
        public async Task GetAllTicketsAsync_ManagerRole_FiltersByDepartment()
        {
            var queryParams = new TicketQueryParams();
            var claims = new UserClaims { Role = "Manager", DeptId = 3, UserId = "manager-1" };
            TicketQueryParams? captured = null;
            var expected = new List<TicketResponseDTO>();

            _ticketRepository.Setup(x => x.GetAllAsync(It.IsAny<TicketQueryParams>()))
                .Callback<TicketQueryParams>(p => captured = p)
                .ReturnsAsync(expected);

            var result = await _ticketService.GetAllTicketsAsync(queryParams, claims);

            result.Should().BeEquivalentTo(expected);
            captured.Should().NotBeNull();
            captured!.DepartmentId.Should().Be(3);
            captured.ReporterId.Should().BeNull();
        }

        [Fact]
        public async Task GetAllTicketsAsync_UserRole_FiltersByReporter()
        {
            var queryParams = new TicketQueryParams();
            var claims = new UserClaims { Role = "User", DeptId = 2, UserId = "user-1" };
            TicketQueryParams? captured = null;
            var expected = new List<TicketResponseDTO>();

            _ticketRepository.Setup(x => x.GetAllAsync(It.IsAny<TicketQueryParams>()))
                .Callback<TicketQueryParams>(p => captured = p)
                .ReturnsAsync(expected);

            var result = await _ticketService.GetAllTicketsAsync(queryParams, claims);

            result.Should().BeEquivalentTo(expected);
            captured.Should().NotBeNull();
            captured!.ReporterId.Should().Be("user-1");
            captured.DepartmentId.Should().BeNull();
        }

        #endregion

        #region UpdateTicketPriority

        [Fact]
        public async Task UpdateTicketPriority_WithValidData_CreatesAuditLogAndUpdatesPriority()
        {
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.Id, 7)
                .With(t => t.priority, TicketPriority.Low)
                .Create();
            var newPriority = (int)TicketPriority.High;
            var userId = "user-1";
            TicketAuditLog? capturedAudit = null;

            _ticketAuditLogRepo.Setup(x => x.CreateAsync(It.IsAny<TicketAuditLog>()))
                .Callback<TicketAuditLog>(log => capturedAudit = log)
                .ReturnsAsync(new TicketAuditLog());
            _ticketRepository.Setup(x => x.UpdatePriority(ticket.Id, newPriority)).Returns(Task.CompletedTask);

            var act = async () => await _ticketService.UpdateTicketPriority(ticket, newPriority, userId);

            await act.Should().NotThrowAsync();
            _ticketAuditLogRepo.Verify(x => x.CreateAsync(It.IsAny<TicketAuditLog>()), Times.Once);
            _ticketRepository.Verify(x => x.UpdatePriority(ticket.Id, newPriority), Times.Once);
            capturedAudit.Should().NotBeNull();
            capturedAudit!.ChangedById.Should().Be(userId);
            capturedAudit.OldValue.Should().Be(TicketPriority.Low.ToString());
            capturedAudit.NewValue.Should().Be(TicketPriority.High.ToString());
        }

        #endregion
    }
}
