using Microsoft.Extensions.DependencyInjection;
using ParserAgent.Parsers;
using ParserAgent.Parsers.Interfaces;

Console.WriteLine("Checking Dev tools instalation...");
Microsoft.Playwright.Program.Main(new[] { "install" });
Console.WriteLine("Dev tools instaled.");

var services = new ServiceCollection();

services.AddScoped<IParser, CoinMarketAppParser>();

using var serviceProvider = services.BuildServiceProvider();

var parser = serviceProvider.GetRequiredService<IParser>();

await parser.Parse("https://coinmarketcap.com/all/views/all/");