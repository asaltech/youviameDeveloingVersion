using Newtonsoft.Json;

namespace youviame.API.UserContext {
    public class FacebookProfile {
        public string Id { get; set; }
        public Location Location { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public string About { get; set; }
        public Albums Albums { get; set; }

        public ProfilePicture Picture { get; set; }
    }
}