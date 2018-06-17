namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class systems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Planets",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        SystemId = c.String(maxLength: 36),
                        Name = c.String(maxLength: 255),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
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
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Planets", "Star_Id", "dbo.Stars");
            DropIndex("dbo.Planets", new[] { "Star_Id" });
            DropTable("dbo.Stars");
            DropTable("dbo.Planets");
        }
    }
}
