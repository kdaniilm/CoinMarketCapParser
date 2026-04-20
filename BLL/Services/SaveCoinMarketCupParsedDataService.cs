using BLL.Models;
using BLL.Services.Interfaces;
using Core.DatabaseManager;
using Microsoft.Data.Sqlite;
using System.Data;

namespace BLL.Services
{
    public class SaveCoinMarketCupParsedDataService : ISaveParsedDataService
    {
        private readonly IDatabaseManager _databaseManager;

        public SaveCoinMarketCupParsedDataService(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task SaveParsedDataAsync(List<CoinMarketCapParsedDTO> parsedData)
        {
            using var connection = new SqliteConnection(_databaseManager.ConnectionString);
            await connection.OpenAsync();

            using (var pragmaCmd = connection.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA journal_mode = WAL; PRAGMA synchronous = NORMAL;";
                await pragmaCmd.ExecuteNonQueryAsync();
            }

            using var transaction = connection.BeginTransaction();

            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;

                command.CommandText = @"
                    INSERT INTO CoinMarketCup 
                    (ParsedDate, Rank, Name, Symbol, MarketCap, Price, CirculatingSupply, Volume24h, Percent24h) 
                    VALUES ($date, $rank, $name, $symbol, $mc, $price, $cs, $v24, $p24)";

                var pDate = command.Parameters.Add("$date", SqliteType.Text);
                var pRank = command.Parameters.Add("$rank", SqliteType.Integer);
                var pName = command.Parameters.Add("$name", SqliteType.Text);
                var pSymbol = command.Parameters.Add("$symbol", SqliteType.Text);
                var pMc = command.Parameters.Add("$mc", SqliteType.Real);
                var pPrice = command.Parameters.Add("$price", SqliteType.Real);
                var pCs = command.Parameters.Add("$cs", SqliteType.Text);
                var pV24 = command.Parameters.Add("$v24", SqliteType.Real);
                var pP24 = command.Parameters.Add("$p24", SqliteType.Real);

                foreach (var item in parsedData)
                {
                    pDate.Value = item.ParsedDate;
                    pRank.Value = item.Rank;
                    pName.Value = item.Name ?? (object)DBNull.Value;
                    pSymbol.Value = item.Symbol ?? (object)DBNull.Value;
                    pMc.Value = item.MarketCap;
                    pPrice.Value = item.Price;
                    pCs.Value = item.CirculatingSupply ?? (object)DBNull.Value;
                    pV24.Value = item.Volume24h;
                    pP24.Value = item.Percent24h;

                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static DataTable ToDataTable(List<CoinMarketCapParsedDTO> parsedData)
        {
            var table = new DataTable();

            table.Columns.Add("ParsedDate", typeof(DateTime));
            table.Columns.Add("Rank", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Symbol", typeof(string));
            table.Columns.Add("MarketCap", typeof(decimal));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("CirculatingSupply", typeof(long));
            table.Columns.Add("Volume24h", typeof(long));
            table.Columns.Add("Percent24h", typeof(decimal));

            foreach (var item in parsedData)
            {
                table.Rows.Add(
                    item.ParsedDate,
                    item.Rank,
                    item.Name ?? string.Empty,
                    item.Symbol,
                    item.MarketCap,
                    item.Price,
                    item.CirculatingSupply ?? (object)DBNull.Value,
                    item.Volume24h ?? (object)DBNull.Value,
                    item.Percent24h
                );
            }

            return table;
        }
    }
}
