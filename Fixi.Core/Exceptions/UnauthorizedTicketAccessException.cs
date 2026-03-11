using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Exceptions
{
    public class UnauthorizedTicketAccessException: Exception
    {

        public UnauthorizedTicketAccessException() : base()
        {
        }
        public UnauthorizedTicketAccessException(string? message) : base(message)
        {
        }
        public UnauthorizedTicketAccessException(string? message, Exception? innerException) : base(message,innerException)
        {
        }
    }
}
