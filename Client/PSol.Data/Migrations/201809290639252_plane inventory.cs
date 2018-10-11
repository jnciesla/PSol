namespace PSol.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class planeinventory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventories", "Planet_Id", c => c.String(maxLength: 36));
            CreateIndex("dbo.Inventories", "Planet_Id");
            AddForeignKey("dbo.Inventories", "Planet_Id", "dbo.Planets", "Id");
            DropColumn("dbo.Users", "Sprite");
            DropColumn("dbo.Users", "Map");
            DropColumn("dbo.Users", "Dir");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Dir", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Map", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Sprite", c => c.Int(nullable: false));
            DropForeignKey("dbo.Inventories", "Planet_Id", "dbo.Planets");
            DropIndex("dbo.Inventories", new[] { "Planet_Id" });
            DropColumn("dbo.Inventories", "Planet_Id");
        }
    }
}
