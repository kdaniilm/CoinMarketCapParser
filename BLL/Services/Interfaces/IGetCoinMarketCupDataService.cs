using BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services.Interfaces
{
    public interface IGetCoinMarketCupDataService
    {
        public Task<List<CoinMarketCapParsedDTO>> GetCoinMarketCupDataAsync(GetCoinMarketCupDataOptions options);
    }
}
