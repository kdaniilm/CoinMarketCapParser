using BLL.Models;
using BLL.Services.Interfaces;
using Core.DatabaseManager;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class GetCoinMarketCupDataService: IGetCoinMarketCupDataService
    {
        private readonly IDatabaseManager _databaseManager;

        public GetCoinMarketCupDataService(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<List<CoinMarketCapParsedDTO>> GetCoinMarketCupDataAsync(GetCoinMarketCupDataOptions options)
        {
            {
                var sql = new StringBuilder(@"
                    SELECT
                        ParsedDate,
                        Rank,
                        Name,
                        Symbol,
                        MarketCap,
                        Price,
                        CirculatingSupply,
                        Volume24h,
                        Percent24h
                    FROM CoinMarketDb.dbo.CoinMarketCup
                    WHERE 1=1
                ");

                var parameters = new List<SqlParameter>();

                void Add(string condition, string name, object value)
                {
                    sql.AppendLine($" AND {condition}");
                    parameters.Add(new SqlParameter(name, value));
                }

                if (!string.IsNullOrWhiteSpace(options.Name))
                    Add("Name LIKE @Name", "@Name", $"%{options.Name}%");

                if (!string.IsNullOrWhiteSpace(options.Symbol))
                    Add("Symbol LIKE @Symbol", "@Symbol", $"%{options.Symbol}%");

                if (options.DateFilter?.From != null)
                    Add("ParsedDate >= @FromDate", "@FromDate", options.DateFilter.From);

                if (options.DateFilter?.To != null)
                    Add("ParsedDate <= @ToDate", "@ToDate", options.DateFilter.To);

                if (options.RankFilter?.From != null)
                    Add("[Rank] >= @FromRank", "@FromRank", options.RankFilter.From);

                if (options.RankFilter?.To != null)
                    Add("[Rank] <= @ToRank", "@ToRank", options.RankFilter.To);

                if (options.MarketCapFilter?.From != null)
                    Add("MarketCap >= @FromMarketCap", "@FromMarketCap", options.MarketCapFilter.From);

                if (options.MarketCapFilter?.To != null)
                    Add("MarketCap <= @ToMarketCap", "@ToMarketCap", options.MarketCapFilter.To);

                if (options.PriceFilter?.From != null)
                    Add("Price >= @FromPrice", "@FromPrice", options.PriceFilter.From);

                if (options.PriceFilter?.To != null)
                    Add("Price <= @ToPrice", "@ToPrice", options.PriceFilter.To);

                if (options.CirculatingSupplyFilter?.From != null)
                    Add("CirculatingSupply >= @FromCirculatingSupply", "@FromCirculatingSupply", options.CirculatingSupplyFilter.From);

                if (options.CirculatingSupplyFilter?.To != null)
                    Add("CirculatingSupply <= @ToCirculatingSupply", "@ToCirculatingSupply", options.CirculatingSupplyFilter.To);

                if (options.Volume24hFilter?.From != null)
                    Add("Volume24h >= @FromVolume24h", "@FromVolume24h", options.Volume24hFilter.From);

                if (options.Volume24hFilter?.To != null)
                    Add("Volume24h <= @ToVolume24h", "@ToVolume24h", options.Volume24hFilter.To);

                if (options.Percent24hFilter?.From != null)
                    Add("Percent24h >= @FromPercent24h", "@FromPercent24h", options.Percent24hFilter.From);

                if (options.Percent24hFilter?.To != null)
                    Add("Percent24h <= @ToPercent24h", "@ToPercent24h", options.Percent24hFilter.To);

                var orderByOrderString = options.OrderByDescending ? "DESC" : "ASC";
                sql.AppendLine($" ORDER BY {options.OrderBy} {orderByOrderString} ");

                var hasOffset = options.Offset.HasValue && options.Offset.Value >= 0;
                var hasLimit = options.Limit.HasValue && options.Limit.Value >= 0;

                if (hasOffset)
                {
                    var limit = options.Limit.HasValue ? options.Limit.Value : long.MaxValue;
                    sql.AppendLine($" OFFSET {options.Offset!.Value} ROWS FETCH NEXT {limit} ROWS ONLY ");
                }
                else if(!hasOffset && hasLimit)
                {
                    sql.Replace("SELECT", $"SELECT TOP ({options.Limit!.Value})");
                }

                var result = new List<CoinMarketCapParsedDTO>();

                using var connection = new SqlConnection(_databaseManager.ConnectionString);
                await connection.OpenAsync();

                using var cmd = new SqlCommand(sql.ToString(), connection);
                cmd.Parameters.AddRange(parameters.ToArray());

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new CoinMarketCapParsedDTO
                    {
                        ParsedDate = reader.GetDateTime(reader.GetOrdinal("ParsedDate")),
                        Rank = reader.GetInt32(reader.GetOrdinal("Rank")),
                        Name = reader["Name"] as string,
                        Symbol = reader["Symbol"] as string,
                        MarketCap = reader.GetDecimal(reader.GetOrdinal("MarketCap")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        CirculatingSupply = reader["CirculatingSupply"] as long?,
                        Volume24h = reader["Volume24h"] as long?,
                        Percent24h = reader.GetDecimal(reader.GetOrdinal("Percent24h"))
                    });
                }

                return result;
            }
        }
    }
}
