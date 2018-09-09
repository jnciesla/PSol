namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mobnames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mobs", "Name", c => c.String(maxLength: 255));
            AddColumn("dbo.Mobs", "Special", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mobs", "Special");
            DropColumn("dbo.Mobs", "Name");
        }
    }
}
