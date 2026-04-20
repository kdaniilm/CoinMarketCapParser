using BLL.Models;
using BLL.Services.Interfaces;
using Core.DatabaseManager;
using Microsoft.Data.Sqlite;
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
            var sql = new StringBuilder($@"
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
                FROM CoinMarketCup
                WHERE 1=1
            ");

            var parameters = new List<SqliteParameter>();

            void Add(string condition, string name, object value, SqliteType? type = null)
            {
                sql.AppendLine($" AND {condition}");
                var parameter = new SqliteParameter(name, value ?? DBNull.Value);
                if (type.HasValue) parameter.SqliteType = type.Value;
                parameters.Add(parameter);
            }

            if (!string.IsNullOrWhiteSpace(options.Name))
                Add("Name LIKE @Name", "@Name", $"%{options.Name}%");

            if (!string.IsNullOrWhiteSpace(options.Symbol))
                Add("Symbol LIKE @Symbol", "@Symbol", $"%{options.Symbol}%");

            if (options.DateFilter?.From != null)
                Add("ParsedDate >= @FromDate", "@FromDate", options.DateFilter.From.ToString("yyyy-MM-dd HH:mm:ss"), SqliteType.Text);

            if (options.DateFilter?.To != null)
                Add("ParsedDate <= @ToDate", "@ToDate", options.DateFilter.To.ToString("yyyy-MM-dd HH:mm:ss"), SqliteType.Text);

            if (options.RankFilter?.From != null)
                Add("Rank >= @FromRank", "@FromRank", options.RankFilter.From, SqliteType.Integer);

            if (options.RankFilter?.To != null)
                Add("Rank <= @FromRank", "@FromRank", options.RankFilter.To, SqliteType.Integer);

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
            sql.AppendLine($" ORDER BY \"{options.OrderBy}\" {orderByOrderString} ");

            if (options.Limit.HasValue || options.Offset.HasValue)
            {
                var limit = options.Limit ?? -1;
                sql.AppendLine($" LIMIT {limit}");

                if (options.Offset.HasValue)
                {
                    sql.AppendLine($" OFFSET {options.Offset.Value}");
                }
            }

            var result = new List<CoinMarketCapParsedDTO>();

            using var connection = new SqliteConnection(_databaseManager.ConnectionString);
            await connection.OpenAsync();

            using var cmd = new SqliteCommand(sql.ToString(), connection);
            cmd.Parameters.AddRange(parameters.ToArray());

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new CoinMarketCapParsedDTO
                {
                    ParsedDate = reader.GetDateTime(reader.GetOrdinal("ParsedDate")),
                    Rank = reader.GetInt32(reader.GetOrdinal("Rank")),
                    Name = reader["Name"]?.ToString(),
                    Symbol = reader["Symbol"]?.ToString(),
                    MarketCap = reader.GetDecimal(reader.GetOrdinal("MarketCap")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    CirculatingSupply = reader.IsDBNull(reader.GetOrdinal("CirculatingSupply")) ? null : reader.GetInt64(reader.GetOrdinal("CirculatingSupply")),
                    Volume24h = reader.IsDBNull(reader.GetOrdinal("Volume24h")) ? null : reader.GetInt64(reader.GetOrdinal("Volume24h")),
                    Percent24h = reader.GetDecimal(reader.GetOrdinal("Percent24h"))
                });
            }

            return result;
        }
    }
}
