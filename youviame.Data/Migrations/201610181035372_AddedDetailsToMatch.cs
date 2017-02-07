namespace youviame.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDetailsToMatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Date", c => c.Double(nullable: false));
            AddColumn("dbo.Matches", "MatchMakerMessage", c => c.String());
            AddColumn("dbo.Matches", "Place", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "Place");
            DropColumn("dbo.Matches", "MatchMakerMessage");
            DropColumn("dbo.Matches", "Date");
        }
    }
}
