using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.ServicesContracts.Abstractions
{
    public interface IBackgroundJobService
    {
        void EnqueueEmail(string to, string subject, string body);

    }
}
