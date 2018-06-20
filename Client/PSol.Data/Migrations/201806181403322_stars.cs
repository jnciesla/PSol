namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stars : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Planets", name: "Star_Id", newName: "StarId");
            RenameIndex(table: "dbo.Planets", name: "IX_Star_Id", newName: "IX_StarId");
            DropColumn("dbo.Planets", "SystemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Planets", "SystemId", c => c.String(maxLength: 36));
            RenameIndex(table: "dbo.Planets", name: "IX_StarId", newName: "IX_Star_Id");
            RenameColumn(table: "dbo.Planets", name: "StarId", newName: "Star_Id");
        }
    }
}
