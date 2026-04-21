using BLL.Models;
using BLL.Services.Interfaces;
using Microsoft.Playwright;
using ParserAgent.Parsers.Interfaces;
using SQLitePCL;
using System.Globalization;

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
                    
                Console.WriteLine("Start parsing rows...");
                var rows = page.Locator("table tbody tr");
                var parsedRows = new List<CoinMarketCapParsedDTO>();
                var prevRowsCount = 0;

                do
                {
                    var rowsCount = await rows.CountAsync();
                    Console.WriteLine($"Rows loaded: {rowsCount}");

                    for (int i = prevRowsCount; i < rowsCount; i++)
                    {
                        Console.WriteLine($"Start parsing row: {i}");
                        var row = rows.Nth(i);

                        if (await IsUnLoadedAsync(row))
                        {
                            Console.WriteLine($"Scrolling on row {i}...");

                            await WaitForLoad(rows, i);
                        }

                        var parsed = ParseRow(await row.InnerTextAsync());

                        if (parsed == null)
                            Console.WriteLine($"Row have incorrect format: {row}");
                        else
                            parsedRows.Add(parsed);
                    }

                    prevRowsCount = rowsCount;

                    Console.WriteLine("Clicking \"Load More\" button...");
                
                    if (loadMoreButton != null && (await loadMoreButton.CountAsync() > 0))
                    {
                        await loadMoreButton!.ClickAsync();
                
                        await page.WaitForFunctionAsync(
                            @"(prev) => document.querySelectorAll('table tbody tr').length > prev",
                            rowsCount
                        );
                    }
                    else
                        Console.WriteLine("\"Load More\" button not found.");
                } while (await loadMoreButton!.IsVisibleAsync());

                await _saveParsedDataService.SaveParsedDataAsync(parsedRows);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{typeof(CoinMarketAppParser).ToString()} threw unexpected exception: {e.ToString()}");
                throw;
            }
        }

        private async Task WaitForLoad(ILocator rows, int index)
        {
            var start = DateTime.UtcNow;
            var timeoutMs = 5000;
            var row = rows.Nth(index);

            while (true)
            {
                try
                {
                    await row.ScrollIntoViewIfNeededAsync();
                }
                catch
                {
                    row = rows.Nth(index);
                }

                if (!await IsUnLoadedAsync(row))
                    break;

                if ((DateTime.UtcNow - start).TotalMilliseconds > timeoutMs)
                    break;

                await Task.Delay(100);
            }
        }

        private static async Task<bool> IsUnLoadedAsync(ILocator row)
        {
            var innerText = await row.InnerTextAsync();

            if (string.IsNullOrWhiteSpace(innerText))
                return false;

            var parts = SplitRowContent(innerText);

            return parts.Count < 10;
        }

        private static CoinMarketCapParsedDTO? ParseRow(string rowContent)
        {
            if (string.IsNullOrWhiteSpace(rowContent))
                return null;

            var parts = SplitRowContent(rowContent);

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

        private static List<string> SplitRowContent(string rowContent)
        {
            return rowContent
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        private static decimal ParseDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            input = input.Trim();

            input = new string(input
                .Where(c => char.IsDigit(c) || c == '.' || c == ',')
                .ToArray());

            if (input.Contains('.') && input.Contains(','))
            {
                if (input.LastIndexOf('.') > input.LastIndexOf(','))
                    input = input.Replace(",", "");
                else
                    input = input.Replace(".", "").Replace(',', '.');
            }
            else
            {
                input = input.Replace(',', '.');
            }

            return decimal.TryParse(
                input,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var result)
                ? result
                : 0;
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
