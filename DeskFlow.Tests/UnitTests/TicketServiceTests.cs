using AutoFixture;
using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.Domain.IdentityEntity;
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Core.DTOs.shared;
using DeskFlow.Core.DTOs.TicketDTOs;
using DeskFlow.Core.Enums;
using DeskFlow.Core.Exceptions;
using DeskFlow.Core.Services;
using DeskFlow.Core.ServicesContracts;
using DeskFlow.Core.ServicesContracts.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace DeskFlow.Tests.UnitTests
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
            _unitOfWork.Setup(x => x.SLASetting.GetByPriorityAsync(ticketDTO.Priority)).ReturnsAsync((SLASetting)null);


            var act = async  () => await _ticketService.CreateTicketAsync(ticketDTO);
             await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreteTicket_WithValidData_AddTicket()
        {
            var ticketDTO = _fixture.Create<CreateTicketDTO>();
            var SLA = _fixture.Create<SLASetting>();
            TicketUsersEmails ticketUsersEmails = new TicketUsersEmails
            {
                ManagerEmail = "manager@example.com",
                TechnicianEmail = "tech@example.com",
                ReporterEmail = "reporter",

            };
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


            _unitOfWork.Setup(x =>  x.SLASetting.GetByPriorityAsync(It.IsAny<int>())).ReturnsAsync(SLA);
            _unitOfWork.Setup(x => x.Ticket.CreateAsync(It.IsAny<Ticket>())).ReturnsAsync(ticket);
            _unitOfWork.Setup(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>())).ReturnsAsync(audit);
            _unitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _unitOfWork.Setup(x => x.Ticket.GetTicketUsersEmailsAsync(It.IsAny<int>())).ReturnsAsync(ticketUsersEmails);
            _backgroundJob.Setup(x => x.EnqueueEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));


            var act = async  () => await _ticketService.CreateTicketAsync(ticketDTO);
             await act.Should().NotThrowAsync();
            _unitOfWork.Verify(x => x.Ticket.CreateAsync(It.IsAny<Ticket>()),Times.Once);
            _unitOfWork.Verify(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>()),Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(),Times.Once);
            _backgroundJob.Verify(x => x.EnqueueEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Once);


        }

        #endregion

        #region UpdateTicket

        [Fact]
        public async Task UpdateTicket_WithWrongId_ThrowNotFoundException()
        {
            Ticket UpdateDTO = _fixture.Create<Ticket>();
            _unitOfWork.Setup(x =>  x.Ticket.GetTicketAsync(UpdateDTO.Id)).ReturnsAsync((TicketDTO)null);

            var act = async  () => await _ticketService.UpdateTicketAsync(UpdateDTO);
            await act.Should().ThrowAsync<TicketNotFoundException>();
        }

        [Fact]
        public async Task UpdateTicket_ValidData_UpdateTicket()
        {
            Ticket UpdateDTO = _fixture.Create<Ticket>();
            TicketDTO ticketDTO = _fixture.Create<TicketDTO>();

            _currentUserService.Setup(x => x.UserId).Returns("userId");
            _unitOfWork.Setup(x =>  x.Ticket.GetTicketAsync(UpdateDTO.Id)).ReturnsAsync(ticketDTO);
            _unitOfWork.Setup(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>())).ReturnsAsync(new TicketAuditLog());
            _unitOfWork.Setup(x => x.Ticket.UpdateAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);

            var act = async  () => await _ticketService.UpdateTicketAsync(UpdateDTO);
            await act.Should().NotThrowAsync();
            _unitOfWork.Verify(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>()),Times.Once);
            _unitOfWork.Verify(x => x.Ticket.UpdateAsync(It.IsAny<Ticket>()),Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(),Times.Once);


        }



        #endregion

        #region AssignTechnician

        [Fact]
        public async Task AssignTechnician_WithMissingTicket_ThrowsTicketNotFoundException()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-1";

            _unitOfWork.Setup(x => x.Ticket.GetTicketAsync(ticketId)).ReturnsAsync((TicketDTO)null);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId);

            await act.Should().ThrowAsync<TicketNotFoundException>();
        }

        [Fact]
        public async Task AssignTechnician_TechnicianAssignsOther_ThrowsBusinessRuleViolationException()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-2";
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.DepartmentId, 1)
                .Create();

            _currentUserService.Setup(x => x.UserId).Returns("tech-1");
            _currentUserService.Setup(x => x.Role).Returns(RoleEnum.Technician);
            _unitOfWork.Setup(x => x.Ticket.GetTicketAsync(ticketId)).ReturnsAsync(ticket);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId);

            await act.Should().ThrowAsync<BusinessRuleViolationException>();
        }

        [Fact]
        public async Task AssignTechnician_WithMissingTechnician_ThrowsNotFoundException()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-1";
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.DepartmentId, 1)
                .Create();

            _unitOfWork.Setup(x => x.Ticket.GetTicketAsync(ticketId)).ReturnsAsync(ticket);
            _identityService.Setup(x => x.FindByIdAsync(newTechnicianId)).ReturnsAsync((ApplicationUser)null);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId);

            await act.Should().ThrowAsync<NotFoundException>();
            _identityService.Verify(x => x.FindByIdAsync(newTechnicianId), Times.Once);
            _unitOfWork.Verify(x => x.Ticket.GetTicketAsync(ticketId), Times.Once);
        }

        [Fact]
        public async Task AssignTechnician_WithTechnicianFromDifferentDepartment_ThrowsBusinessRuleViolationException()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-1";
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.DepartmentId, 1)
                .Create();
            var technician = new ApplicationUser { DepartmentId = 2, FullName = "Tech User" };

            _unitOfWork.Setup(x => x.Ticket.GetTicketAsync(ticketId)).ReturnsAsync(ticket);
            _identityService.Setup(x => x.FindByIdAsync(newTechnicianId)).ReturnsAsync(technician);
            _currentUserService.Setup(x => x.UserId).Returns("tech-1");
            _currentUserService.Setup(x => x.Role).Returns(RoleEnum.Technician);
            _currentUserService.Setup(x => x.DeptId).Returns(1);    

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId);

            await act.Should().ThrowAsync<BusinessRuleViolationException>();
        }

        [Fact]
        public async Task AssignTechnician_WithValidData_AssignsTechnicianAndCreatesAuditLog()
        {
            var ticketId = 1;
            var newTechnicianId = "tech-1";
            var ticket = _fixture.Build<TicketDTO>()
                .With(t => t.DepartmentId, 1)
                .With(t => t.AssignedToFullname, (string?)null)
                .Create();
            var technician = new ApplicationUser { DepartmentId = 1, FullName = "Tech User" };

            _currentUserService.Setup(x => x.UserId).Returns("tech-1");
            _currentUserService.Setup(x => x.Role).Returns(RoleEnum.Technician);
            _currentUserService.Setup(x => x.DeptId).Returns(1);

            _unitOfWork.Setup(x => x.Ticket.GetTicketAsync(ticketId)).ReturnsAsync(ticket);
            _identityService.Setup(x => x.FindByIdAsync(newTechnicianId)).ReturnsAsync(technician);
            _unitOfWork.Setup(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>())).ReturnsAsync(new TicketAuditLog());
            _unitOfWork.Setup(x => x.Ticket.AssignTechnician(ticketId, newTechnicianId, "tech-1")).Returns(Task.CompletedTask);

            var act = async () => await _ticketService.AssignTechnician(ticketId, newTechnicianId);

            await act.Should().NotThrowAsync();
            _unitOfWork.Verify(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>()), Times.Once);
            _unitOfWork.Verify(x => x.Ticket.AssignTechnician(ticketId, newTechnicianId, "tech-1"), Times.Once);
        }

        #endregion

        #region GetTicketById

        [Fact]
        public async Task GetTicketByIdAsync_WithValidId_ReturnsTicket()
        {
            var ticketId = 10;
            var expected = _fixture.Create<TicketFullResponseDTO>();

            _unitOfWork.Setup(x => x.Ticket.GetFullTicketAsync(ticketId)).ReturnsAsync(expected);

            var result = await _ticketService.GetTicketByIdAsync(ticketId);

            result.Should().BeEquivalentTo(expected);
            _unitOfWork.Verify(x => x.Ticket.GetFullTicketAsync(ticketId), Times.Once);
        }

        #endregion

        #region DeleteTicket

        [Fact]
        public async Task DeleteTicket_WithMissingTicket_ThrowsTicketNotFoundException()
        {
            var ticketId = 5;
            var userId = "user-1";

            _unitOfWork.Setup(x => x.Ticket.GetTicketAsync(ticketId)).ReturnsAsync((TicketDTO)null);
            _currentUserService.Setup(x => x.UserId).Returns(userId);

            var act = async () => await _ticketService.DeleteTicket(ticketId);

            await act.Should().ThrowAsync<TicketNotFoundException>();
        }

        [Fact]
        public async Task DeleteTicket_WithValidTicket_CreatesAuditLogAndDeletes()
        {
            var ticketId = 5;
            var userId = "user-1";
            var ticket = _fixture.Create<TicketDTO>();

            _unitOfWork.Setup(x => x.Ticket.GetTicketAsync(ticketId)).ReturnsAsync(ticket);
            _unitOfWork.Setup(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>())).ReturnsAsync(new TicketAuditLog());
            _unitOfWork.Setup(x => x.Ticket.DeleteAsync(ticketId)).Returns(Task.CompletedTask);
            _currentUserService.Setup(x => x.UserId).Returns(userId);

            var act = async () => await _ticketService.DeleteTicket(ticketId);

            await act.Should().NotThrowAsync();
            _unitOfWork.Verify(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>()), Times.Once);
            _unitOfWork.Verify(x => x.Ticket.DeleteAsync(ticketId), Times.Once);
        }

        #endregion

        #region GetAllTickets

        [Fact]
        public async Task GetAllTicketsAsync_ManagerRole_FiltersByDepartment()
        {
            var queryParams = new TicketQueryParams();
            TicketQueryParams? captured = null;
            var expected = new List<TicketResponseDTO>();

            _currentUserService.Setup(x => x.Role).Returns(RoleEnum.Manager);
            _currentUserService.Setup(x => x.DeptId).Returns(3);
            _currentUserService.Setup(x => x.UserId).Returns("manager");

            _unitOfWork.Setup(x => x.Ticket.GetAllAsync(It.IsAny<TicketQueryParams>()))
                .Callback<TicketQueryParams>(p => captured = p)
                .ReturnsAsync(expected);

            var result = await _ticketService.GetAllTicketsAsync(queryParams);

            result.Should().BeEquivalentTo(expected);
            captured.Should().NotBeNull();
            captured!.DepartmentId.Should().Be(3);
            captured.ReporterId.Should().BeNull();
        }

        [Fact]
        public async Task GetAllTicketsAsync_UserRole_FiltersByReporter()
        {
            var queryParams = new TicketQueryParams();
            TicketQueryParams? captured = null;
            var expected = new List<TicketResponseDTO>();

            _currentUserService.Setup(x => x.Role).Returns(RoleEnum.User);
            _currentUserService.Setup(x => x.DeptId).Returns(2);
            _currentUserService.Setup(x => x.UserId).Returns("user-1");

            _unitOfWork.Setup(x => x.Ticket.GetAllAsync(It.IsAny<TicketQueryParams>()))
                .Callback<TicketQueryParams>(p => captured = p)
                .ReturnsAsync(expected);

            var result = await _ticketService.GetAllTicketsAsync(queryParams);

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


            _unitOfWork.Setup(x => x.Ticket.GetTicketAsync(ticket.Id)).ReturnsAsync(ticket);

            _unitOfWork.Setup(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>()))
                .ReturnsAsync(new TicketAuditLog());
            _unitOfWork.Setup(x => x.Ticket.UpdatePriority(ticket.Id, newPriority,userId)).Returns(Task.CompletedTask);
            _unitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);


            var act = async () => await _ticketService.UpdateTicketPriority(ticket.Id, newPriority);

            await act.Should().NotThrowAsync();
            _unitOfWork.Verify(x => x.TicketAuditLog.CreateAsync(It.IsAny<TicketAuditLog>()), Times.Once);
            _unitOfWork.Verify(x => x.Ticket.UpdatePriority(ticket.Id, newPriority,userId), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);

        }

        #endregion
    }
}
