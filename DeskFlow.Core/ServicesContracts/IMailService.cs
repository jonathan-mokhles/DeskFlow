using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.ServicesContracts
{
    public interface IMailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
