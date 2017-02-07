using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using youviame.Data.Context;
using youviame.Data.Enitities;

namespace youviame.Data.Repositories {
    public class MatchRepository : IMatchRepository {
        private readonly IYouviameContext _context;

        public MatchRepository(IYouviameContext context) {
            _context = context;
        }

        public Match Get(Guid id) {
            return _context.Matches
                .Include(x => x.DatePerson1)
                .Include(x => x.DatePerson2)
                .Include(x => x.MatchMaker)
                .FirstOrDefault(x => x.Id.Equals(id));
        }

        public IEnumerable<Match> GetAll() {
            return _context.Matches
                .Include(x => x.DatePerson1)
                .Include(x => x.DatePerson2)
                .Include(x => x.MatchMaker);
        }

        public void Save(Match entity) {
            _context.Matches.Add(entity);
            _context.SaveChanges();
        }

        public Task SaveAsync(Match entity) {
            throw new NotImplementedException();
        }

        public void Save(List<Match> entities) {
            _context.Matches.AddRange(entities);
            _context.SaveChanges();
        }

        public void Update(Match entity) {
            _context.Matches.Attach(entity);
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
            _context.SaveChanges();
        }

        public Task UpdateAsync(Match entity) {
            throw new NotImplementedException();
        }

        public Match Get(Guid dateperson1Id, Guid dateperson2Id) {
            var match = _context.Matches.FirstOrDefault(
                x => x.DatePerson1.Id.Equals(dateperson1Id) && x.DatePerson2.Id.Equals(dateperson2Id));
            if (match == null) {
                match =
                    _context.Matches.FirstOrDefault(
                        x => x.DatePerson1.Id.Equals(dateperson2Id) && x.DatePerson2.Id.Equals(dateperson1Id));
            }

            return match;
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
    }
}