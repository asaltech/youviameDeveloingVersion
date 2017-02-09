using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using youviame.Data.Context;
using youviame.Data.Enitities;

namespace youviame.Data.Repositories {
    public class UserRepository : IUserRepository {
        private readonly IYouviameContext _context;
        public UserRepository(IYouviameContext context) {
            _context = context;
        }

        public User Get(Guid id) {
           
            return _context.Users.Find(id);
        }

        public IEnumerable<User> GetAll() {
            return _context.Users;
        }

        public User Get(string facebookId) {
            var firstOrDefault = _context.Users.FirstOrDefault(x => x.FacebookId.Equals(facebookId));
            return firstOrDefault;
        }

        public IEnumerable<User> Get(IEnumerable<string> facebookIds) {
            var users = new List<User>();
            foreach (var facebookId in facebookIds) {
                var user = _context.Users.FirstOrDefault(x => x.FacebookId == facebookId);
                if(user != null)
                    users.Add(user);
            }
            return users;
        }

        public async Task SaveAsync(User entity) {
            var existingUser = _context.Users.FirstOrDefault(x => x.FacebookId.Equals(entity.FacebookId));
            if (existingUser != null) {
                _context.Entry(existingUser).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return;
            }
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
        }
        public void Save(User entity) {
            
            _context.Users.Add(entity);
            _context.SaveChanges();
        }

        public void Save(List<User> entities) {
            throw new NotImplementedException();
        }

        public void Update(User entity) {
            _context.Users.Attach(entity);
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task UpdateAsync(User entity) {
            _context.Users.Attach(entity);
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
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
