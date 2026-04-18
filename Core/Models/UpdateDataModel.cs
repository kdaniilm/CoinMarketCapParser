using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class UpdateDataModel
    {
        public string? Table { get; set; }
        public Guid? Id { get; set; }
        public Dictionary<string, object> ColumnDataDictionary { get; set; }
    }
}
