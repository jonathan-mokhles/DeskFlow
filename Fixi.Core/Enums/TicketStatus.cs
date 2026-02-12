using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Enums
{
    public enum TicketStatus
    {
        New = 1,
        Assigned = 2,
        InProgress = 3,
        Resolved = 4,
        Closed = 5,
        Rejected = 6,
        OnHold = 7,
        Canceled = 8
    }
}
