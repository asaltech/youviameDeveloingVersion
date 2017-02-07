using System.Data.Entity;
using youviame.Data.Context;

namespace youviame.Data.Migrations
{
    using System.Data.Entity.Migrations;
    public sealed class Configuration : DbMigrationsConfiguration<YouviameContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "youviame.Data.YouviameContext.youviame.Database.ConnectionString";
            
        }

        protected override void Seed(YouviameContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
