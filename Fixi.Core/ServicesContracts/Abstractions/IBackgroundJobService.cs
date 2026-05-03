using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.ServicesContracts.Abstractions
{
    public interface IBackgroundJobService
    {
        void SendEmail(string to, string subject, string body);

    }
}
