namespace youviame.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStatusToMatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "MatchStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "MatchStatus");
        }
    }
}
