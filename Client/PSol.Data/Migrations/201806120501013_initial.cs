namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        Login = c.String(maxLength: 255),
                        Password = c.String(maxLength: 255),
                        Name = c.String(maxLength: 255),
                        MaxHealth = c.Int(nullable: false),
                        Health = c.Int(nullable: false),
                        MaxShield = c.Int(nullable: false),
                        Shield = c.Int(nullable: false),
                        Sprite = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        Exp = c.Int(nullable: false),
                        Map = c.Int(nullable: false),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                        Dir = c.Int(nullable: false),
                        Rotation = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
