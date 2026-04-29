using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.ServicesContracts
{
    public interface IMailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
