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
                await page.GotoAsync(url);

                var loadMoreButton = page.Locator("button:has-text('Load More')");
                var rows = page.Locator("table tbody tr");

                if (loadMoreButton == null)
                    throw new ElementNotFoundException("\"Load More\" button not found");

                while (await loadMoreButton.IsVisibleAsync())
                {
                    Console.WriteLine($"Rows loaded: {await rows.CountAsync()}");
                    Console.WriteLine("Clicking \"Load More\" button...");

                    var countBefore = await rows.CountAsync();

                    await loadMoreButton.ClickAsync();

                    await page.WaitForFunctionAsync(
                        @"(prev) => document.querySelectorAll('table tbody tr').length > prev",
                        countBefore
                    );
                }

                Console.WriteLine("Start parsing rows...");
                var allRows = await rows.AllAsync();
                var parsedRows = new List<CoinMarketCapParsedDTO>();

                foreach (var row in allRows)
                {
                    await row.ScrollIntoViewIfNeededAsync();
                    var rowInnerHtml = await row.InnerHTMLAsync();

                    var parsedRow = ParseRow(rowInnerHtml);
                    parsedRows.Add(parsedRow);

                    Console.WriteLine($"Rows parsed: {parsedRows.Count} of {await rows.CountAsync()}...");
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

        private static CoinMarketCapParsedDTO ParseRow(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml($"<tr>{html}</tr>");

            var tds = doc.DocumentNode.SelectNodes("//td");

            var result = new CoinMarketCapParsedDTO();
            result.ParsedDate = DateTime.Now;
            result.Rank = int.Parse(tds[0].InnerText.Trim());
            result.Name = tds[1].SelectSingleNode(".//a[contains(@class,'--name')]")?.InnerText.Trim();
            result.Symbol = tds[1].SelectSingleNode(".//a[contains(@class,'--symbol')]")?.InnerText.Trim();
            result.MarketCap = ParseDecimal(tds[3].SelectSingleNode(".//span[@data-nosnippet]").InnerText.Trim());
            result.Price = ParseDecimal(tds[4].InnerText.Trim());
            result.CirculatingSupply = ParseInt64(tds[5].InnerText.Trim());
            result.Volume24h = ParseInt64(tds[6].InnerText.Trim());
            result.Percent24h = ParseDecimal(tds[8].InnerText.Trim());

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
