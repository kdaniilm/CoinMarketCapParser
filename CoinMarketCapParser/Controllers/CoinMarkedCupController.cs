using BLL.Models;
using Core.DatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoinMarkedCupController : ControllerBase
    {
        private readonly IDatabaseManager _databaseManager;

        public CoinMarkedCupController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [HttpGet]
        [Route("get-data")]
        public List<CoinMarketCapParsedDTO> GetCoinMarketCapParsedData()
        {
            return new List<CoinMarketCapParsedDTO>();
        }
    }
}
