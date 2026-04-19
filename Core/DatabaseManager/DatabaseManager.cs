using Microsoft.Data.SqlClient;

namespace Core.DatabaseManager
{
    public class DatabaseManager : IDatabaseManager
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public DatabaseManager()
        {
            ConnectionString = "Server=.;Integrated Security=true;TrustServerCertificate=True;";
            DatabaseName = "CoinMarketDb";
        }

        public async Task CreateDatabaseIfNotExistsAsync()
        {
            var script = await GetScriptAsync("CreateDatabase");

            var batches = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            foreach (var batch in batches)
            {
                using var command = new SqlCommand(batch, connection);
                command.CommandTimeout = 60;
                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task<string> GetScriptAsync(string scriptFileName)
        {
            var scriptPath = Path.GetFullPath($@"{AppDomain.CurrentDomain.BaseDirectory}..\..\..\..\Core\Scripts\{scriptFileName}.sql");
            var script = await File.ReadAllTextAsync(scriptPath);

            return script;
        }
    }
}
