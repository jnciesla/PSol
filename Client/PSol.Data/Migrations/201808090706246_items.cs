namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class items : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Items");
        }
    }
}
