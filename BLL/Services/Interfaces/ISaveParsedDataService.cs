using BLL.Models;

namespace BLL.Services.Interfaces
{
    public interface ISaveParsedDataService
    {
        Task SaveParsedDataAsync(List<CoinMarketCapParsedDTO> parsedData);
    }
}
