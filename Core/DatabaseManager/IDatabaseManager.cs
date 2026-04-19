using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DatabaseManager
{
    public interface IDatabaseManager
    {
        public Task CreateDatabaseIfNotExists();
        public Task InsertData(InsertDataModel insertDataModel);
        public Task UpdateData(UpdateDataModel updateDataModel);
        public Task<object> GetData(GetDataModel getDataModel);
    }
}
