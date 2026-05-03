using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Exceptions
{
    public class UnauthorizedOperationException: Exception
    {
        public UnauthorizedOperationException(string message) : base(message)
        {
        }
    }
}
