using System;
namespace TechObjektBackend.Models
{
	public class FilterCondition
	{
        public string ColumnName { get; set; }
        public string FilterType { get; set; }
        public string Operator { get; set; }
        public string Type { get; set; }
        public string Index { get; set; }
    }
}

