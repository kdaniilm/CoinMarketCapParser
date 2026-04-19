using BLL.Models;
using BLL.Services.Interfaces;
using Core.Exceptions;
using Core.Models;
using HtmlAgilityPack;
using Microsoft.Playwright;
using ParserAgent.Parsers.Interfaces;

namespace ParserAgent.Parsers
{
    public class CoinMarketAppParser : IParser
    {
        private readonly ISaveParsedDataService _saveParsedDataService;

        public CoinMarketAppParser(ISaveParsedDataService saveParsedDataService)
        {
            _saveParsedDataService = saveParsedDataService;
        }

        public async Task Parse(string url)
        {
            try
            {
                Console.WriteLine("Launching browser...");
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

                Console.WriteLine("Creating the page...");
                var page = await browser.NewPageAsync();
                await page.SetViewportSizeAsync(1920, 1080);

                Console.WriteLine($"Navigating to {url}...");
                await page.RouteAsync("**/*", route =>
                {
                    var type = route.Request.ResourceType;

                    if (type == "image" ||
                        type == "stylesheet" ||
                        type == "font")
                    {
                        return route.AbortAsync();
                    }

                    return route.ContinueAsync();
                });
                await page.GotoAsync(url);

                Console.WriteLine($"Searching for \"Load More\" button...");
                var loadMoreButton = page.Locator("button:has-text('Load More')");

                var isLoadMoreButtonExists = loadMoreButton != null && (await loadMoreButton.CountAsync() > 0);

                if (!isLoadMoreButtonExists)
                    Console.WriteLine("\"Load More\" button not found.");

                Console.WriteLine("Start loading rows to parse...");
                var rows = page.Locator("table tbody tr");

                if (isLoadMoreButtonExists)
                {
                    while (await loadMoreButton!.IsVisibleAsync())
                    {
                        var rowsCount = await rows.CountAsync();

                        Console.WriteLine($"Rows loaded: {rowsCount}");
                        Console.WriteLine("Clicking \"Load More\" button...");

                        await loadMoreButton.ClickAsync();

                        await page.WaitForFunctionAsync(
                            @"(prev) => document.querySelectorAll('table tbody tr').length > prev",
                            rowsCount
                        );
                    }
                }

                var total = await rows.CountAsync();

                if (total == 0) {
                    Console.WriteLine("No rows to parse.");
                    return;
                }

                Console.WriteLine($"Total loaded rows count: {total}.");

                Console.WriteLine("Start parsing rows...");

                var parsedRows = new List<CoinMarketCapParsedDTO>();

                for (int i = 0; i < total; i++)
                {
                    Console.WriteLine($"Scrolling on row {i}...");
                    await rows.Nth(i).ScrollIntoViewIfNeededAsync();

                    var row = rows.Nth(i);

                    var parsed = ParseRow(await row.InnerTextAsync());

                    if (parsed == null)
                        Console.WriteLine($"Row have incorrect format: {row}");
                    else
                        parsedRows.Add(parsed);

                    Console.WriteLine($"Parsed {i}/{total}");
                }

                Console.WriteLine("All rows parsed.");

                await _saveParsedDataService.SaveParsedDataAsync(parsedRows);
            }
            catch(ParsingException pe)
            {
                Console.WriteLine($"Parsing exception {pe.ToString()}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{typeof(CoinMarketAppParser).ToString()} threw unexpected exception: {e.ToString()}");
                throw;
            }
        }

        private static CoinMarketCapParsedDTO? ParseRow(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            var parts = raw
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (parts.Count < 10)
                return null;

            var result = new CoinMarketCapParsedDTO
            {
                ParsedDate = DateTime.Now,

                Rank = int.Parse(parts[0]),
                Name = parts[1],
                Symbol = parts[2],

                MarketCap = ParseDecimal(parts[3]),
                Price = ParseDecimal(parts[4]),

                CirculatingSupply = ParseInt64(parts[5]),
                Volume24h = ParseInt64(parts[6]),

                Percent24h = ParseDecimal(parts[8]),
            };

            return result;
        }

        private static decimal ParseDecimal(string decimalString)
        {
            try
            {
                decimalString = new string(decimalString.Where(c => char.IsDigit(c) || c == '.').ToArray());

                var isResultParsed = decimal.TryParse(decimalString, out var result);

                return isResultParsed ? result : 0;
            }
            catch(Exception) { return 0; }
        }

        private static Int64? ParseInt64(string int64String)
        {
            try
            {
                int64String = new string(int64String.Where(c => char.IsDigit(c)).ToArray());

                var isResultParsed = Int64.TryParse(int64String, out var result);

                return isResultParsed ? result : 0;
            } catch(Exception) { return 0; }
        }
    }
}
