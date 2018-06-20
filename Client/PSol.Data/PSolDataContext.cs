using System.Data.Entity;
using PSol.Data.Models;

namespace PSol.Data
{
    public class PSolDataContext : DbContext
    {
        private const int MaxIdLength = 36;
        public DbSet<User> Users { get; set; }
        public DbSet<Star> Stars { get; set; }
        public DbSet<Planet> Planets { get; set; }
        public DbSet<MobType> MobTypes { get; set; }
        public DbSet<Mob> ActiveMobs { get; set; }

        public PSolDataContext() : base("PSolDataConnection")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PSolDataContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Properties<string>().Configure(c => c.HasMaxLength((255)));

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Star>()
                .Property(u => u.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Planet>()
                .Property(u => u.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Planet>()
                .Property(u => u.SystemId)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<MobType>()
                .Property(m => m.Id)
                .HasMaxLength(MaxIdLength);

            modelBuilder.Entity<Mob>()
                .Property(m => m.Id)
                .HasMaxLength(MaxIdLength);
        }
    }
}
