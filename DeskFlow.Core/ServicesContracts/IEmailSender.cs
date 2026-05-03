using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.ServicesContracts
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
