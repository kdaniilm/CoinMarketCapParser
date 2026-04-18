using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class ElementNotFoundException: ParsingException
    {
        public ElementNotFoundException(string? message = null): base(message) { }
    }
}
