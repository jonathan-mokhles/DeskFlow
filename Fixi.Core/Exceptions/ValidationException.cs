using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Exceptions
{
    public class ValidationException :Exception
    {
        public ValidationException(string errors): base(errors)
        {
        }
    }
}
