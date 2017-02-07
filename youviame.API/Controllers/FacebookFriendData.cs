using System.Collections.Generic;
using Newtonsoft.Json;

namespace youviame.API.Controllers {
    public class FacebookFriendData {
        [JsonProperty("data")]
        public List<FacebookFriend> Friends { get; set; }

        [JsonProperty("paging")]
        public PagingData Paging { get; set; }

        public FacebookFriendData() {
            Friends = new List<FacebookFriend>();
            Paging = new PagingData { Cursors = new Cursors() };
        }
    }

    public class PagingData {
        [JsonProperty("cursors")]
        public Cursors Cursors { get; set; }

    }

    public class Cursors {
        [JsonProperty("after")]
        public string After { get; set; }

        [JsonProperty("before")]
        public string Before { get; set; }
    }

    public class FacebookFriend {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("installed")]
        public bool Installed { get; set; }
    }
}