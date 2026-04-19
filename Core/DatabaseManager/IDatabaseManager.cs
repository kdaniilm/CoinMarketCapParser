using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DatabaseManager
{
    public interface IDatabaseManager
    {
        public string ConnectionString { get; }
        public Task CreateDatabaseIfNotExistsAsync();
    }
}
