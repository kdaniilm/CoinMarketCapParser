using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DatabaseManager
{
    public interface IDatabaseManager
    {
        public void CreateDatabase(string databaseName);
        public void InsertData(InsertDataModel insertDataModel);
        public void UpdateData(UpdateDataModel updateDataModel);
        public void GetData(GetDataModel getDataModel);
    }
}
