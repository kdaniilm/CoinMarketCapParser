using ParserAgent.Parsers.Interfaces;
using Microsoft.Playwright;

namespace ParserAgent.Parsers
{
    public class CoinMarketAppParser : IParser
    {
        public CoinMarketAppParser()
        {
        }

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

                var html = await page.ContentAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{typeof(CoinMarketAppParser).ToString()} throw exception: {ex.ToString()}");
                throw;
            }
        }
    }
}
