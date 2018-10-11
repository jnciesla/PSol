namespace PSol.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class improveduserstats : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Rank", c => c.String(maxLength: 255));
            AddColumn("dbo.Users", "Hull", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Armor", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Thrust", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Power", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Recharge", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Repair", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Defense", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Offense", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Capacity", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Weapons", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Weapons");
            DropColumn("dbo.Users", "Capacity");
            DropColumn("dbo.Users", "Offense");
            DropColumn("dbo.Users", "Defense");
            DropColumn("dbo.Users", "Repair");
            DropColumn("dbo.Users", "Recharge");
            DropColumn("dbo.Users", "Power");
            DropColumn("dbo.Users", "Thrust");
            DropColumn("dbo.Users", "Armor");
            DropColumn("dbo.Users", "Hull");
            DropColumn("dbo.Users", "Rank");
        }
    }
}
