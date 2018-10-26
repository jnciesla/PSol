namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        UserId = c.String(maxLength: 36),
                        ItemId = c.String(maxLength: 36),
                        Slot = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Dropped = c.DateTime(nullable: false),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                        Planet_Id = c.String(maxLength: 36),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Planets", t => t.Planet_Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.Planet_Id);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        Name = c.String(maxLength: 255),
                        Type = c.String(maxLength: 255),
                        Description = c.String(maxLength: 255),
                        Image = c.Int(nullable: false),
                        Color = c.Int(nullable: false),
                        Mass = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                        Stack = c.Boolean(nullable: false),
                        Level = c.Int(nullable: false),
                        Slot = c.Int(nullable: false),
                        Hull = c.Int(nullable: false),
                        Shield = c.Int(nullable: false),
                        Armor = c.Int(nullable: false),
                        Thrust = c.Int(nullable: false),
                        Power = c.Int(nullable: false),
                        Damage = c.Int(nullable: false),
                        Recharge = c.Int(nullable: false),
                        Repair = c.Int(nullable: false),
                        Defense = c.Int(nullable: false),
                        Offense = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Weapons = c.Int(nullable: false),
                        Special = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Mobs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        MobTypeId = c.String(maxLength: 36),
                        Health = c.Single(nullable: false),
                        Shield = c.Single(nullable: false),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                        Rotation = c.Single(nullable: false),
                        SpawnDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        KilledDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Alive = c.Boolean(nullable: false),
                        Name = c.String(maxLength: 255),
                        Special = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MobTypes", t => t.MobTypeId)
                .Index(t => t.MobTypeId);
            
            CreateTable(
                "dbo.MobTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        Name = c.String(maxLength: 255),
                        MaxHealth = c.Int(nullable: false),
                        MaxShield = c.Int(nullable: false),
                        Sprite = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        BonusExp = c.Int(nullable: false),
                        Credits = c.Int(nullable: false),
                        MaxSpawned = c.Int(nullable: false),
                        SpawnTimeMin = c.Int(nullable: false),
                        SpawnTimeMax = c.Int(nullable: false),
                        SpawnRadius = c.Int(nullable: false),
                        Star_Id = c.String(maxLength: 36),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stars", t => t.Star_Id)
                .Index(t => t.Star_Id);
            
            CreateTable(
                "dbo.Stars",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        Name = c.String(maxLength: 255),
                        Belligerence = c.String(maxLength: 255),
                        Class = c.String(maxLength: 255),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Planets",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        StarId = c.String(maxLength: 36),
                        Name = c.String(maxLength: 255),
                        Sprite = c.Int(nullable: false),
                        Color = c.Int(nullable: false),
                        Belligerence = c.String(maxLength: 255),
                        Class = c.String(maxLength: 255),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                        Orbit = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stars", t => t.StarId)
                .Index(t => t.StarId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        Login = c.String(maxLength: 255),
                        Password = c.String(maxLength: 255),
                        Rank = c.String(maxLength: 255),
                        Name = c.String(maxLength: 255),
                        MaxHealth = c.Int(nullable: false),
                        Health = c.Int(nullable: false),
                        MaxShield = c.Int(nullable: false),
                        Shield = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        Exp = c.Int(nullable: false),
                        Credits = c.Int(nullable: false),
                        Armor = c.Int(nullable: false),
                        Thrust = c.Int(nullable: false),
                        Power = c.Int(nullable: false),
                        Recharge = c.Int(nullable: false),
                        Repair = c.Int(nullable: false),
                        Defense = c.Int(nullable: false),
                        Offense = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Weapons = c.Int(nullable: false),
                        Weap1Charge = c.Int(nullable: false),
                        Weap2Charge = c.Int(nullable: false),
                        Weap3Charge = c.Int(nullable: false),
                        Weap4Charge = c.Int(nullable: false),
                        Weap5Charge = c.Int(nullable: false),
                        Weap1ChargeRate = c.Int(nullable: false),
                        Weap2ChargeRate = c.Int(nullable: false),
                        Weap3ChargeRate = c.Int(nullable: false),
                        Weap4ChargeRate = c.Int(nullable: false),
                        Weap5ChargeRate = c.Int(nullable: false),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                        Rotation = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Inventories", "UserId", "dbo.Users");
            DropForeignKey("dbo.Mobs", "MobTypeId", "dbo.MobTypes");
            DropForeignKey("dbo.MobTypes", "Star_Id", "dbo.Stars");
            DropForeignKey("dbo.Planets", "StarId", "dbo.Stars");
            DropForeignKey("dbo.Inventories", "Planet_Id", "dbo.Planets");
            DropIndex("dbo.Planets", new[] { "StarId" });
            DropIndex("dbo.MobTypes", new[] { "Star_Id" });
            DropIndex("dbo.Mobs", new[] { "MobTypeId" });
            DropIndex("dbo.Inventories", new[] { "Planet_Id" });
            DropIndex("dbo.Inventories", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.Planets");
            DropTable("dbo.Stars");
            DropTable("dbo.MobTypes");
            DropTable("dbo.Mobs");
            DropTable("dbo.Items");
            DropTable("dbo.Inventories");
        }
    }
}
