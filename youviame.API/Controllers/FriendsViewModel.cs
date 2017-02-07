using System.Collections.Generic;

namespace youviame.API.Controllers {
    public class FriendsViewModel {
        public IEnumerable<UserViewModel> Friends { get; set; }
        public IEnumerable<UserViewModel> FriendsOfFriends { get; set; }

    }
}