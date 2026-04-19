Application serve to parse, save and get saved coins list from https://coinmarketcap.com/all/views/all/. As main Framework used .NET 10, for API used ASP.NET Core 10, for site data parsing used Microsoft.Playwright library.

App builds with moduled architecture, were:
1. Core - classLibrary to work with db, stores DatabaseManager, database identical models and scripts to create or update database.
2. BLL - classLibrary to provide busines logic for Api and Parser.
3. API - ASP.NET Core project with enabled swagger to get filtered data from database.
4. ParserAgent - Console application to parse data from https://coinmarketcap.com/all/views/all/ and fuly independed from API.
