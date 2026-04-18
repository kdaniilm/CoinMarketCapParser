using Core.Exceptions;
using Microsoft.Playwright;
using ParserAgent.Parsers.Interfaces;

namespace ParserAgent.Parsers
{
    public class CoinMarketAppParser : IParser
    {
        public async Task Parse(string url)
        {
            try
            {
                Console.WriteLine("Launching browser...");
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
                Console.WriteLine("Browser launched.");

                Console.WriteLine("Creating the page...");
                var page = await browser.NewPageAsync();
                Console.WriteLine("Page created.");

                Console.WriteLine($"Navigating to {url}...");
                await page.GotoAsync(url);
                Console.WriteLine($"Navigated to {url}.");

                var loadMoreButton = page.Locator("button:has-text('Load More')");
                var rows = page.Locator("table tbody tr");

                if (loadMoreButton == null)
                    throw new ElementNotFoundException("\"Load More\" button not found");

                while (await loadMoreButton.IsVisibleAsync())
                {
                    Console.WriteLine("Clicking \"Load More\" button...");

                    var countBefore = await rows.CountAsync();

                    await loadMoreButton.ClickAsync();

                    await page.WaitForFunctionAsync(
                        @"(prev) => document.querySelectorAll('table tbody tr').length > prev",
                        countBefore
                    );

                    Console.WriteLine($"Rows loaded: {await rows.CountAsync()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{typeof(CoinMarketAppParser).ToString()} throw exception: {ex.ToString()}");
                throw;
            }
        }
    }
}
