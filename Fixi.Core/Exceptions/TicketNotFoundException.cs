using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Exceptions
{
    public class TicketNotFoundException : Exception
    {
        public TicketNotFoundException() : base()
        {
        }
        public TicketNotFoundException(string? message) : base(message)
        {
        }
        public TicketNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
