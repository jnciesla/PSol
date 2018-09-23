namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class equipment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "iHull", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iDrive", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iShield", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iArmor", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iComputer", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iPayload", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iWeapon1", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iWeapon2", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iWeapon3", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iWeapon4", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iWeapon5", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iAmmo1", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iAmmo2", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "iAmmo3", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "qAmmo1", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "qAmmo2", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "qAmmo3", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "qAmmo3");
            DropColumn("dbo.Users", "qAmmo2");
            DropColumn("dbo.Users", "qAmmo1");
            DropColumn("dbo.Users", "iAmmo3");
            DropColumn("dbo.Users", "iAmmo2");
            DropColumn("dbo.Users", "iAmmo1");
            DropColumn("dbo.Users", "iWeapon5");
            DropColumn("dbo.Users", "iWeapon4");
            DropColumn("dbo.Users", "iWeapon3");
            DropColumn("dbo.Users", "iWeapon2");
            DropColumn("dbo.Users", "iWeapon1");
            DropColumn("dbo.Users", "iPayload");
            DropColumn("dbo.Users", "iComputer");
            DropColumn("dbo.Users", "iArmor");
            DropColumn("dbo.Users", "iShield");
            DropColumn("dbo.Users", "iDrive");
            DropColumn("dbo.Users", "iHull");
        }
    }
}
