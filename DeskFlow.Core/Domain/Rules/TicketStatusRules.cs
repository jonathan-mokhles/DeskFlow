using System;
using System.Collections.Generic;
using System.Text;
using DeskFlow.Core.Enums;

namespace DeskFlow.Core.Domain.Rules
{
    public class TicketStatusRules
    {
        public static List<TransitionRule> TransitionRules = new List<TransitionRule>()
        {
            new TransitionRule { From = TicketStatus.Open, To = TicketStatus.InProgress, Role = RoleEnum.Technician },
            new TransitionRule { From = TicketStatus.Open, To = TicketStatus.Canceled, Role = RoleEnum.User },
            new TransitionRule { From = TicketStatus.InProgress, To = TicketStatus.Resolved, Role = RoleEnum.Technician },
            new TransitionRule { From = TicketStatus.InProgress, To = TicketStatus.OnHold, Role = RoleEnum.Technician },
            new TransitionRule { From = TicketStatus.OnHold, To = TicketStatus.InProgress, Role = RoleEnum.Technician },
            new TransitionRule { From = TicketStatus.OnHold, To = TicketStatus.Canceled, Role = RoleEnum.User },
            new TransitionRule { From = TicketStatus.Resolved, To = TicketStatus.Closed, Role = RoleEnum.User },
            new TransitionRule { From = TicketStatus.Resolved, To = TicketStatus.InProgress, Role = RoleEnum.User }
        };
    }
}
