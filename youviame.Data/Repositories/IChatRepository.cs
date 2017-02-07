using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using youviame.Data.Context;
using youviame.Data.Enitities;

namespace youviame.Data.Repositories {
    public interface IChatRepository : IRepository<ChatMessage> {
        IEnumerable<ChatMessage> Get(Guid matchId);

    }

    public class ChatRepository : IChatRepository {
        private readonly IYouviameContext _context;

        public ChatRepository(IYouviameContext context) {
            _context = context;
        }
       

        public ChatMessage Get(Guid id) {
            throw new NotImplementedException();
        }

        IEnumerable<ChatMessage> IChatRepository.Get(Guid matchId) {
            return _context.ChatMessages.Where(x => x.MatchId == matchId);
        }

        public IEnumerable<ChatMessage> GetAll() {
            throw new NotImplementedException();
        }

        public void Save(ChatMessage entity) {
            throw new NotImplementedException();
        }

        public async Task SaveAsync(ChatMessage entity) {
            _context.ChatMessages.Add(entity);
            await _context.SaveChangesAsync();
        }

        public void Save(List<ChatMessage> entities) {
            throw new NotImplementedException();
        }

        public void Update(ChatMessage entity) {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ChatMessage entity) {
            throw new NotImplementedException();
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