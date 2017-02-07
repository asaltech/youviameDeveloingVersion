using System.Collections.Generic;
using youviame.Data.Enitities;

namespace youviame.Data.Repositories {
    public interface IUserRepository : IRepository<User> {
        User Get(string facebookId);
        IEnumerable<User> Get(IEnumerable<string> facebookIds);
    }
}