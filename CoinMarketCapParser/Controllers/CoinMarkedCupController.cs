using BLL.Models;
using BLL.Services.Interfaces;
using Core.DatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoinMarkedCupController : ControllerBase
    {
        private readonly IGetCoinMarketCupDataService _getCoinMarketCupDataService;

        public CoinMarkedCupController(IGetCoinMarketCupDataService getCoinMarketCupDataService)
        {
            _getCoinMarketCupDataService = getCoinMarketCupDataService;
        }

        [HttpGet]
        [Route("get-data")]
        public async Task<List<CoinMarketCapParsedDTO>> GetCoinMarketCapParsedData([FromQuery]GetCoinMarketCupDataOptions options)
        {
            return await _getCoinMarketCupDataService.GetCoinMarketCupDataAsync(options);
        }
    }
}
