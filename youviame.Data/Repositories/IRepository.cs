using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using youviame.Data.Enitities;

namespace youviame.Data.Repositories {
    public interface IRepository<T> : IDisposable where T : BaseEntity {
        T Get(Guid id);
        IEnumerable<T> GetAll();
        void Save(T entity);

        Task SaveAsync(T entity);

        void Save(List<T> entities);

        void Update(T entity);

        Task UpdateAsync(T entity);
    }
}