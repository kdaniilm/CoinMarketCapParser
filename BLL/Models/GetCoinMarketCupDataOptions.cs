using System;
using System.Collections.Generic;
using System.Text;

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

        public Filter<DateTime>? DateFilter {  get; set; }

        public Filter<int>? RankFilter { get; set; }

        public Filter<decimal>? MarketCapFilter { get; set; }

        public Filter<decimal>? PriceFilter { get; set; }

        public Filter<long>? CirculatingSupplyFilter { get; set; }

        public Filter<long>? Volume24hFilter { get; set; }

        public Filter<decimal>? Percent24hFilter { get; set; }
    }

    public class Filter<T>
    {
        public T? From { get; set; }
        public T? To { get; set; }
    }
}
