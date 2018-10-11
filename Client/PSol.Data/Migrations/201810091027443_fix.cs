namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventories", "Charge", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            
            DropColumn("dbo.Inventories", "Charge");
        }
    }
}
