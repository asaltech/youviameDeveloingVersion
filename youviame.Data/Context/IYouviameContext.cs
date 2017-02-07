using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using youviame.Data.Enitities;

namespace youviame.Data.Context {
    public interface IYouviameContext : IDisposable {
        DbSet<User> Users { get; set; }
        DbSet<Match> Matches { get; set; }
        DbSet<ChatMessage> ChatMessages { get; set; }
        DbSet<Report> Reports { get; set; }
        DbSet<Log> Logs { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbSet<TEntity> Set<TEntity>()
            where TEntity : class;
    }
}