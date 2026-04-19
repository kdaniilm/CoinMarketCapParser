using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Models
{
    public class GetCoinMarketCupDataOptions
    {
        public int? Offset {  get; set; }
        public int? Limit { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? FromRank { get; set; }
        public int? ToRank { get; set; }

        public string? Name { get; set; }
        public string? Symbol { get; set; }

        public decimal? FromMarketCap {  get; set; }
        public decimal? ToMarketCap { get; set; }

        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }

        public long? FromCirculatingSupply { get; set; }
        public long? ToCirculatingSupply { get; set; }

        public long? FromVolume24h { get; set; }
        public long? ToVolume24h { get; set; }

        public decimal? FromPercent24h { get; set; }
        public decimal? ToPercent24h { get; set; }

        public string OrderBy { get; set; } = "ParsedDate";
        public bool OrderByDescending { get; set; } = false;
    }
}
