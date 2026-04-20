namespace BLL.Models
{
    public class GetCoinMarketCupDataOptions
    {
        public int? Offset {  get; set; }
        public int? Limit { get; set; }

        public string? Name { get; set; }
        public string? Symbol { get; set; }

        public string OrderBy { get; set; } = "ParsedDate";
        public bool OrderByDescending { get; set; } = false;

        public Option<DateTime>? DateFilter {  get; set; }

        public Option<int>? RankFilter { get; set; }

        public Option<double>? MarketCapFilter { get; set; }

        public Option<double>? PriceFilter { get; set; }

        public Option<long>? CirculatingSupplyFilter { get; set; }

        public Option<long>? Volume24hFilter { get; set; }

        public Option<double>? Percent24hFilter { get; set; }
    }
}
