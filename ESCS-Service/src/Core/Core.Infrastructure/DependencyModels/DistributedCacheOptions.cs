namespace Core.Infrastructure.DependencyModels
{
    public class DistributedCacheOptions
    {
        public string Endpoints { get; set; } = default!;
        public string Password { get; set; } = default!;
        public int Database { get; set; } = default!;
    }
}
