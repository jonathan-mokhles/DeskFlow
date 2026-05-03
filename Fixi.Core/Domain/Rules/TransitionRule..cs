using DeskFkow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Domain.Rules
{
    public class TransitionRule
    {
        public TicketStatus From { get; set; }
        public TicketStatus To { get; set; }
        public RoleEnum Role { get; set; }
    }
}
