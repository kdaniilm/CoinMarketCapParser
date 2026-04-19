namespace Core.DatabaseManager
{
    public interface IDatabaseManager
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }

        public Task CreateDatabaseIfNotExistsAsync();
    }
}
