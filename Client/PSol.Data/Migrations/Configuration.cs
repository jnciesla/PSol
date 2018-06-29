using System.Collections.Generic;
using PSol.Data.Models;

namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PSol.Data.PSolDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PSol.Data.PSolDataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.Stars.AddOrUpdate(new Star
            {
                Id = "7179FC09-EBAA-49D8-8ED4-A53F7B95F260",
                Name = "Vyt",
                X = 1000,
                Y = 1000,
                Planets = new List<Planet>()
            });
            context.Stars.AddOrUpdate(new Star
            {
                Id = "d2f03d3c-c465-4ae2-a887-d783037253ef",
                Name = "Scathe",
                X = 4000,
                Y = 700,
                Planets = new List<Planet>()
            });

            context.SaveChanges();

            context.MobTypes.AddOrUpdate(new MobType
            {
                Id = "7A93241E-D251-4E7E-A565-46B0E274B5C6",
                BonusExp = 0,
                Credits = 0,
                Level = 1,
                MaxHealth = 100,
                MaxShield = 0,
                MaxSpawned = 4,
                Name = "Vyt Scout",
                SpawnRadius = 100,
                SpawnTimeMax = 45,
                SpawnTimeMin = 20,
                Sprite = 2,
                Star = context.Stars.FirstOrDefault(s => s.Name == "Vyt")
            });

            context.SaveChanges();
        }
    }
}
