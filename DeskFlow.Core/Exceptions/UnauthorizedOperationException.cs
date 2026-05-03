using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Exceptions
{
    public class UnauthorizedOperationException: Exception
    {
        public UnauthorizedOperationException(string message) : base(message)
        {
        }
    }
}
