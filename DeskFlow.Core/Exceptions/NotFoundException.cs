using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
