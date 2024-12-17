using Core.Infrastructure.Persistences;
using ESCS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ESCS.Infrastructure.Persistences
{
    public class EscsDbContext : BaseDbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserApiKey> UserApiKeys { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<KeyAllowedEndpoint> KeyAllowedEndpoints { get; set; }
        public DbSet<UserEmailServiceConfiguration> UserEmailServiceConfigurations { get; set; }
        public DbSet<ServiceEndpoint> ServiceEndpoints { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        public EscsDbContext(DbContextOptions opt) : base(opt) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // TODO: Check
            base.OnModelCreating(modelBuilder);
        }
    }
}
