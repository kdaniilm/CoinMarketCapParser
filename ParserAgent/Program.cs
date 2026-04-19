using BLL.Services;
using BLL.Services.Interfaces;
using Core.DatabaseManager;
using Microsoft.Extensions.DependencyInjection;
using ParserAgent.Parsers;
using ParserAgent.Parsers.Interfaces;

Console.WriteLine("Checking Dev tools instalation...");
Microsoft.Playwright.Program.Main(new[] { "install" });
Console.WriteLine("Dev tools instaled.");

var services = new ServiceCollection();
services.AddSingleton<IDatabaseManager, DatabaseManager>();
services.AddScoped<IParser, CoinMarketAppParser>();
services.AddScoped<ISaveParsedDataService, SaveCoinMarketCupParsedDataService>();

using var serviceProvider = services.BuildServiceProvider();

var databasemanager = serviceProvider.GetRequiredService<IDatabaseManager>();
await databasemanager.CreateDatabaseIfNotExistsAsync();

var parser = serviceProvider.GetRequiredService<IParser>();
var parseUrl = args[0] ?? "https://coinmarketcap.com/all/views/all/";
await parser.Parse(parseUrl);