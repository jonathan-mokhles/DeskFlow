using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.ServicesContracts;
using DeskFkow.Core.ServicesContracts.Abstractions;
using Hangfire;

namespace DeskFkow.Infrastructure.BackgroundJobs
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IMailService _mailService;

        public BackgroundJobService(IMailService mailService)
        {
            _mailService = mailService;
        }


        public void SendEmail(string to, string subject, string body)
        {
            BackgroundJob.Enqueue(() => _mailService.SendEmailAsync(to, subject, body));
        }
    }
}
