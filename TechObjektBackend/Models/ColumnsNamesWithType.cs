using System;
namespace TechObjektBackend.Models
{
	public class ColumnsNamesWithType
    {
        public ColumnsNamesWithType(string name, string type) {
            switch (type)
            {
                case "MongoDB.Bson.BsonString":
                    this.CollumnType = "text";
                    break;
                case "MongoDB.Bson.BsonInt32":
                    this.CollumnType = "number";
                    break;
                default:
                    this.CollumnType = "text";
                    break;
            }
            this.CollumnName = name;
            
        }
        public string? CollumnName { get; set; }
        public string? CollumnType { get; set; }
    }
}

