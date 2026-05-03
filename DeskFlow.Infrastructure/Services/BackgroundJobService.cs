using System;
using System.Collections.Generic;
using System.Text;
using DeskFlow.Core.ServicesContracts;
using DeskFlow.Core.ServicesContracts.Abstractions;
using Hangfire;

namespace DeskFlow.Infrastructure.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        IEmailSender _emailSender;
        public BackgroundJobService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public void EnqueueEmail(string to, string subject, string body)
        {
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(to, subject, body));
        }
    }
}
