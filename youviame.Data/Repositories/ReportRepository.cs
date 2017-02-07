using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using youviame.Data.Context;
using youviame.Data.Enitities;

namespace youviame.Data.Repositories {
    public class ReportRepository : IReportRepository {
        private readonly IYouviameContext _context;

        public ReportRepository(IYouviameContext context) {
            _context = context;
        }

      

   

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

        public Report Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Report> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Save(Report entity)
        {
            _context.Reports.Add(entity);
            _context.SaveChanges();
        }

        public Task SaveAsync(Report entity)
        {
            throw new NotImplementedException();
        }

        public void Save(List<Report> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(Report entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Report entity)
        {
            throw new NotImplementedException();
        }
    }
}