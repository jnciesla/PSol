namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mobs2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Mobs", name: "Type_Id", newName: "MobTypeId");
            RenameIndex(table: "dbo.Mobs", name: "IX_Type_Id", newName: "IX_MobTypeId");
            AddColumn("dbo.Mobs", "KilledDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Mobs", "Alive", c => c.Boolean(nullable: false));
            AlterColumn("dbo.MobTypes", "SpawnRadius", c => c.Int(nullable: false));
            DropColumn("dbo.Mobs", "Dir");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mobs", "Dir", c => c.Int(nullable: false));
            AlterColumn("dbo.MobTypes", "SpawnRadius", c => c.Single(nullable: false));
            DropColumn("dbo.Mobs", "Alive");
            DropColumn("dbo.Mobs", "KilledDate");
            RenameIndex(table: "dbo.Mobs", name: "IX_MobTypeId", newName: "IX_Type_Id");
            RenameColumn(table: "dbo.Mobs", name: "MobTypeId", newName: "Type_Id");
        }
    }
}
