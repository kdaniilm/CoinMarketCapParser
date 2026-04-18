using System;
using System.Collections.Generic;
using System.Text;

namespace ParserAgent.Parsers.Interfaces
{
    public interface IParser
    {
        public Task Parse(string url);
    }
}
