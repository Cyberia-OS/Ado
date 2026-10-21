using System.Data.Entity;

namespace CarRentalDbFirst
{
    public class Brands
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }

    public class Cars
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int ReleaseYear { get; set; }
        public decimal PricePerDay { get; set; }
        public int BrandId { get; set; }
        public virtual Brands Brands { get; set; }
    }

    public class CarRentalDbEntities : DbContext
    {
        public CarRentalDbEntities() : base("name=CarRentalDbEntities")
        {
            Database.SetInitializer<CarRentalDbEntities>(null);
        }

        public DbSet<Brands> Brands { get; set; }
        public DbSet<Cars> Cars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cars>()
            .HasRequired(c => c.Brands)
            .WithMany()
            .HasForeignKey(c => c.BrandId);

            base.OnModelCreating(modelBuilder);
        }
    }
}