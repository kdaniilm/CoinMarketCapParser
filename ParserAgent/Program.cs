using ParserAgent.Parsers;

var parser = new CoinMarketAppParser();

await parser.Parse("https://coinmarketapp.com/coins");