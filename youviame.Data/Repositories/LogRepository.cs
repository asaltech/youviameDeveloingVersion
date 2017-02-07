using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using youviame.Data.Context;
using youviame.Data.Enitities;

namespace youviame.Data.Repositories {
    public class LogRepository : ILogRepository {
        private readonly IYouviameContext _context;
        public LogRepository(IYouviameContext context)
        {
            _context = context;
        }

        //insert log line


        private bool _disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (!this._disposed) {
                if (disposing) {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void InsertLog(string message)
        {
            Log entity = new Log();
            entity.LogTime = DateTime.Now;
            entity.LogMessage = message;
            _context.Logs.Add(entity);
            _context.SaveChanges();
        }

        public Log Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Log> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Save(Log entity)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(Log entity)
        {
            throw new NotImplementedException();
        }

        public void Save(List<Log> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(Log entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Log entity)
        {
            throw new NotImplementedException();
        }
    }
}
