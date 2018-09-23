namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class slot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Slot", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "Slot");
        }
    }
}
