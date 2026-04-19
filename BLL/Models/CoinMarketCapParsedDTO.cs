namespace BLL.Models
{
    public class CoinMarketCapParsedDTO
    {
        public DateTime ParsedDate {  get; set; }
        public int Rank { get; set; }
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        public decimal MarketCap { get; set; }
        public decimal Price { get; set; }
        public long? CirculatingSupply { get; set; }
        public long? Volume24h { get; set; }
        public decimal Percent24h { get; set; }
    }
}
