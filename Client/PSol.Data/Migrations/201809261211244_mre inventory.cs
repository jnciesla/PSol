namespace PSol.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class mreinventory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventories", "Dropped", c => c.DateTime(nullable: false));
            AddColumn("dbo.Inventories", "X", c => c.Single(nullable: true));
            AddColumn("dbo.Inventories", "Y", c => c.Single(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inventories", "Y");
            DropColumn("dbo.Inventories", "X");
            DropColumn("dbo.Inventories", "Dropped");
        }
    }
}
