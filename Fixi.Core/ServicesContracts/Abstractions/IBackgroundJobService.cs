using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.ServicesContracts.Abstractions
{
    public interface IBackgroundJobService
    {
        void SendEmail(string to, string subject, string body);

    }
}
