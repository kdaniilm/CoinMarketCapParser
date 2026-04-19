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

                // Date
                if (options.FromDate.HasValue)
                    Add("ParsedDate >= @FromDate", "@FromDate", options.FromDate.Value);

                if (options.ToDate.HasValue)
                    Add("ParsedDate <= @ToDate", "@ToDate", options.ToDate.Value);

                // Rank
                if (options.FromRank.HasValue)
                    Add("[Rank] >= @FromRank", "@FromRank", options.FromRank.Value);

                if (options.ToRank.HasValue)
                    Add("[Rank] <= @ToRank", "@ToRank", options.ToRank.Value);

                // Name (Contains)
                if (!string.IsNullOrWhiteSpace(options.Name))
                    Add("Name LIKE @Name", "@Name", $"%{options.Name}%");

                // Symbol (Contains)
                if (!string.IsNullOrWhiteSpace(options.Symbol))
                    Add("Symbol LIKE @Symbol", "@Symbol", $"%{options.Symbol}%");

                // MarketCap
                if (options.FromMarketCap.HasValue)
                    Add("MarketCap >= @FromMarketCap", "@FromMarketCap", options.FromMarketCap.Value);

                if (options.ToMarketCap.HasValue)
                    Add("MarketCap <= @ToMarketCap", "@ToMarketCap", options.ToMarketCap.Value);

                // Price
                if (options.FromPrice.HasValue)
                    Add("Price >= @FromPrice", "@FromPrice", options.FromPrice.Value);

                if (options.ToPrice.HasValue)
                    Add("Price <= @ToPrice", "@ToPrice", options.ToPrice.Value);

                // CirculatingSupply
                if (options.FromCirculatingSupply.HasValue)
                    Add("CirculatingSupply >= @FromCirculatingSupply", "@FromCirculatingSupply", options.FromCirculatingSupply.Value);

                if (options.ToCirculatingSupply.HasValue)
                    Add("CirculatingSupply <= @ToCirculatingSupply", "@ToCirculatingSupply", options.ToCirculatingSupply.Value);

                // Volume24h
                if (options.FromVolume24h.HasValue)
                    Add("Volume24h >= @FromVolume24h", "@FromVolume24h", options.FromVolume24h.Value);

                if (options.ToVolume24h.HasValue)
                    Add("Volume24h <= @ToVolume24h", "@ToVolume24h", options.ToVolume24h.Value);

                // Percent24h
                if (options.FromPercent24h.HasValue)
                    Add("Percent24h >= @FromPercent24h", "@FromPercent24h", options.FromPercent24h.Value);

                if (options.ToPercent24h.HasValue)
                    Add("Percent24h <= @ToPercent24h", "@ToPercent24h", options.ToPercent24h.Value);

                sql.AppendLine(" ORDER BY ParsedDate DESC ");

                var hasOffset = options.Offset.HasValue && options.Offset.Value >= 0;
                var hasLimit = options.Limit.HasValue && options.Limit.Value >= 0;

                if (hasOffset && hasLimit)
                {
                    sql.AppendLine($" OFFSET {options.Offset!.Value} ROWS FETCH NEXT {options.Limit!.Value} ROWS ONLY ");
                }
                else if (!hasOffset && hasLimit)
                {
                    sql.Replace("SELECT", $"SELECT TOP ({options.Limit!.Value})");
                }
                else if(hasOffset && !hasLimit)
                {
                    sql.Replace("SELECT", $"SELECT LAST ({options.Offset!.Value})");
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
