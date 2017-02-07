using System.Data.Entity;
using youviame.Data.Enitities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;


namespace youviame.Data.Context {
    public class YouviameContext : DbContext, IYouviameContext {
        public YouviameContext() : base("name=youviame.Database.ConnectionString") {
            //Database.SetInitializer<YouviameContext>(new CreateDatabaseIfNotExists<YouviameContext>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<Report> Reports { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
