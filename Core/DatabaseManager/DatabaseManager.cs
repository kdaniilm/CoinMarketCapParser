using Microsoft.Data.Sqlite;

namespace Core.DatabaseManager
{
    public class DatabaseManager : IDatabaseManager
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public DatabaseManager()
        {
            ConnectionString = $@"Data Source={AppDomain.CurrentDomain.BaseDirectory}..\..\..\..\Core\CoinMarketDb.db;";
            DatabaseName = "CoinMarketDb";
        }

        public async Task CreateDatabaseIfNotExistsAsync()
        {
            var script = await GetScriptAsync("CreateDatabase");

            using var connection = new SqliteConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = script;
            command.CommandTimeout = 60;

            await command.ExecuteNonQueryAsync();
        }

        private static async Task<string> GetScriptAsync(string scriptFileName)
        {
            var scriptPath = Path.GetFullPath($@"{AppDomain.CurrentDomain.BaseDirectory}..\..\..\..\Core\Scripts\{scriptFileName}.sql");
            var script = await File.ReadAllTextAsync(scriptPath);

            return script;
        }
    }
}
