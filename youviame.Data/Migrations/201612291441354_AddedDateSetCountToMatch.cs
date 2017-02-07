namespace youviame.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDateSetCountToMatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "DatePerson1DateSetCount", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "DatePerson2DateSetCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "DatePerson2DateSetCount");
            DropColumn("dbo.Matches", "DatePerson1DateSetCount");
        }
    }
}
