using Core.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Core.DatabaseManager
{
    public class DatabaseManager : IDatabaseManager
    {
        public async Task CreateDatabaseIfNotExists()
        {
            var connectionString = "Server=.;Integrated Security=true;TrustServerCertificate=True;";

            var scriptPath = Path.GetFullPath(@"..\..\..\..\Core\Scripts\CreateDatabase.sql");
            var script = await File.ReadAllTextAsync(scriptPath);

            var batches = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            foreach (var batch in batches)
            {
                using var command = new SqlCommand(batch, connection);
                command.CommandTimeout = 60;
                await command.ExecuteNonQueryAsync();
            }
        }

        public Task<object> GetData(GetDataModel getDataModel)
        {
            throw new NotImplementedException();
        }

        public Task InsertData(InsertDataModel insertDataModel)
        {
            throw new NotImplementedException();
        }

        public Task UpdateData(UpdateDataModel updateDataModel)
        {
            throw new NotImplementedException();
        }
    }
}
