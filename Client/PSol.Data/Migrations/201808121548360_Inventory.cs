namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inventory : DbMigration
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
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                //.ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.Inventories", "UserId", "dbo.Users");
            DropIndex("dbo.Inventories", new[] { "UserId" });
            DropTable("dbo.Inventories");
        }
    }
}
