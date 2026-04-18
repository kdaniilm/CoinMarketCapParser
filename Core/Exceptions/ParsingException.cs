using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class ParsingException: Exception
    {
        public ParsingException(string? message = null): base(message) { }
    }
}
