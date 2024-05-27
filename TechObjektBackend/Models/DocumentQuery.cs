using System;
namespace TechObjektBackend.Models
{
	public class DocumentQuery
	{
		public string collectionName { get; set; }

        public List<List<FilterCondition>> FilterConditions { get; set; }


    }
}

