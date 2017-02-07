using System;
using youviame.Data.Enitities;

namespace youviame.Data.Repositories {
    public interface IMatchRepository : IRepository<Match> {
        Match Get(Guid dateperson1Id, Guid dateperson2Id);
    }
}