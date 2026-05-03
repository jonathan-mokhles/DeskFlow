using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Exceptions
{
    public class BusinessRuleViolationException: Exception
    {
        public BusinessRuleViolationException(string message) : base(message)
        {
        }
    }
}
