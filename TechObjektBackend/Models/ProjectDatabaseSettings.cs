using System;
namespace TechObjektBackend.Models
{
	public class ProjectDatabaseSettings
	{
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string PrisonersCollectionName { get; set; } = null!;
    }
}

