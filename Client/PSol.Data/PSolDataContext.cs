using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSol.Data.Models;

namespace PSol.Data
{
    public class PSolDataContext : DbContext
    {
        private const int MaxIdLength = 36;
        public DbSet<User> Users { get; set; }

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
        }
    }
}
