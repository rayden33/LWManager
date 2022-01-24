using System.Data.Entity;

namespace LWManager
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }
        public DbSet<LeaseContract> LeaseContracts { get; set; }
        public DbSet<ArchiveLeaseContract> ArchiveLeaseContracts { get; set; }
        public DbSet<ReturnedLeaseContract> ReturnedLeaseContracts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductReference> ProductReferences { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<ReturnProduct> ReturnProducts { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
