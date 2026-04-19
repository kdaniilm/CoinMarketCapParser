namespace ParserAgent.Parsers.Interfaces
{
    public interface IParser
    {
        public Task Parse(string url);
    }
}
