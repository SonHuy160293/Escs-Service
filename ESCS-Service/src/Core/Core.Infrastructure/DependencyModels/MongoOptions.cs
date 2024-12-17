namespace Core.Infrastructure.DependencyModels
{
    public class MongoOptions
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
