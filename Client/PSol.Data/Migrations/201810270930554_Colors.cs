namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Colors : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Items", "Color", c => c.String(maxLength: 255));
            AlterColumn("dbo.Planets", "Color", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Planets", "Color", c => c.Int(nullable: false));
            AlterColumn("dbo.Items", "Color", c => c.Int(nullable: false));
        }
    }
}
