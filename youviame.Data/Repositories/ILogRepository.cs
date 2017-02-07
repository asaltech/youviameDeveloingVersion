using System;
using youviame.Data.Enitities;

namespace youviame.Data.Repositories {
    public interface ILogRepository : IRepository<Log>
    {
        //Match Get(Guid dateperson1Id, Guid dateperson2Id);

        void InsertLog(string message);
    }
}