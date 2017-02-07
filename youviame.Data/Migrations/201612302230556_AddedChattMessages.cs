namespace youviame.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedChattMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ReporterId = c.Guid(nullable: false),
                        ReportedUser = c.Guid(nullable: false),
                        ReportMessage = c.String(),
                        //DateTime = c.Double(nullable: false),
                        ReportReason = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reports");
        }
    }
}
