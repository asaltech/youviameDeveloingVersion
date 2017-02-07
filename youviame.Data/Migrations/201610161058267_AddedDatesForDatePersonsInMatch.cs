namespace youviame.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDatesForDatePersonsInMatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "DatePerson1Dates", c => c.String());
            AddColumn("dbo.Matches", "DatePerson2Dates", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "DatePerson2Dates");
            DropColumn("dbo.Matches", "DatePerson1Dates");
        }
    }
}
