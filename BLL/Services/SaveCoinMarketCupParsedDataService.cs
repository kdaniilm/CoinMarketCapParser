using BLL.Models;
using BLL.Services.Interfaces;
using Core.DatabaseManager;
using Microsoft.Data.SqlClient;
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
            using var connection = new SqlConnection(_databaseManager.ConnectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                using (var bulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulk.DestinationTableName = "[CoinMarketDb].[dbo].[CoinMarketCup]";

                    bulk.ColumnMappings.Add("ParsedDate", "ParsedDate");
                    bulk.ColumnMappings.Add("Rank", "Rank");
                    bulk.ColumnMappings.Add("Name", "Name");
                    bulk.ColumnMappings.Add("Symbol", "Symbol");
                    bulk.ColumnMappings.Add("MarketCap", "MarketCap");
                    bulk.ColumnMappings.Add("Price", "Price");
                    bulk.ColumnMappings.Add("CirculatingSupply", "CirculatingSupply");
                    bulk.ColumnMappings.Add("Volume24h", "Volume24h");
                    bulk.ColumnMappings.Add("Percent24h", "Percent24h");

                    var table = ToDataTable(parsedData);
                    await bulk.WriteToServerAsync(table);
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
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
