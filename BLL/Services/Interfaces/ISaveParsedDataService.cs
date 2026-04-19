using BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services.Interfaces
{
    public interface ISaveParsedDataService
    {
        Task SaveParsedDataAsync(List<CoinMarketCapParsedDTO> parsedData);
    }
}
