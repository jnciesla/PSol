namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mobs3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Mobs", "SpawnDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Mobs", "KilledDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Mobs", "KilledDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Mobs", "SpawnDate", c => c.DateTime(nullable: false));
        }
    }
}
