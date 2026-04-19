using BLL.Models;

namespace BLL.Services.Interfaces
{
    public interface IGetCoinMarketCupDataService
    {
        public Task<List<CoinMarketCapParsedDTO>> GetCoinMarketCupDataAsync(GetCoinMarketCupDataOptions options);
    }
}
