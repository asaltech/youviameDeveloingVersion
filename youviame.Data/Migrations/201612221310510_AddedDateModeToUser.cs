namespace youviame.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDateModeToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "DateModeEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "DateModeEnabled");
        }
    }
}
