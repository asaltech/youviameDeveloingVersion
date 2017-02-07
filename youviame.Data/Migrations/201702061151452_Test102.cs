namespace youviame.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test102 : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.ChatMessages",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            UserId = c.Guid(nullable: false),
            //            Message = c.String(),
            //            DateTime = c.Double(nullable: false),
            //            MatchId = c.Guid(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LogTime = c.DateTime(nullable: false),
                        LogMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Matches",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            MatchMakerStatus = c.Int(nullable: false),
            //            DatePerson1Status = c.Int(nullable: false),
            //            DatePerson2Status = c.Int(nullable: false),
            //            MatchStatus = c.Int(nullable: false),
            //            DatePerson1Dates = c.String(),
            //            DatePerson2Dates = c.String(),
            //            DatePerson1DateSetCount = c.Int(nullable: false),
            //            DatePerson2DateSetCount = c.Int(nullable: false),
            //            Date = c.Double(nullable: false),
            //            MatchMakerMessage = c.String(),
            //            Place = c.String(),
            //            DatePerson1_Id = c.Guid(),
            //            DatePerson2_Id = c.Guid(),
            //            MatchMaker_Id = c.Guid(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Users", t => t.DatePerson1_Id)
            //    .ForeignKey("dbo.Users", t => t.DatePerson2_Id)
            //    .ForeignKey("dbo.Users", t => t.MatchMaker_Id)
            //    .Index(t => t.DatePerson1_Id)
            //    .Index(t => t.DatePerson2_Id)
            //    .Index(t => t.MatchMaker_Id);
            
            //CreateTable(
            //    "dbo.Users",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false, identity: true),
            //            FacebookId = c.String(),
            //            FirstName = c.String(),
            //            LastName = c.String(),
            //            AboutMe = c.String(),
            //            ProfilePictures = c.String(),
            //            Location = c.String(),
            //            DateModeEnabled = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ReporterId = c.Guid(nullable: false),
                        ReportedUser = c.Guid(nullable: false),
                        ReportMessage = c.String(),
                        ReportReason = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.Matches", "MatchMaker_Id", "dbo.Users");
            //DropForeignKey("dbo.Matches", "DatePerson2_Id", "dbo.Users");
            //DropForeignKey("dbo.Matches", "DatePerson1_Id", "dbo.Users");
            //DropIndex("dbo.Matches", new[] { "MatchMaker_Id" });
            //DropIndex("dbo.Matches", new[] { "DatePerson2_Id" });
            //DropIndex("dbo.Matches", new[] { "DatePerson1_Id" });
            DropTable("dbo.Reports");
            //DropTable("dbo.Users");
            //DropTable("dbo.Matches");
            DropTable("dbo.Logs");
            //DropTable("dbo.ChatMessages");
        }
    }
}
