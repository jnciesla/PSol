namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class planets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Planets", "Sprite", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "Color", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "Orbit", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Planets", "Orbit");
            DropColumn("dbo.Planets", "Color");
            DropColumn("dbo.Planets", "Sprite");
        }
    }
}
