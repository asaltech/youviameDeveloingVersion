using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace youviame.Data.Enitities
{
    public class User : BaseEntity
    {
        public string FacebookId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AboutMe { get; set; }
        public string ProfilePictures { get; set; }
        public string Location { get; set; }

        public bool DateModeEnabled { get; set; }
        public User() {
            
        }

        public static User Create(string facebookId, string firstName, string lastName, string aboutMe, IEnumerable<Image> profilePictures, string location) {
            var user = new User {
                FacebookId = facebookId,
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                AboutMe = aboutMe,
                ProfilePictures = JsonConvert.SerializeObject(profilePictures),
                Location = location,
                DateModeEnabled = true
            };
            return user;
        }

        public void SetLocation(string location) {
            Location = location;
        }

        public void SetAboutMe(string aboutMe) {
            AboutMe = aboutMe;
        }

        public void SetProfilePictures(List<Image> profilePictures) {
            ProfilePictures = JsonConvert.SerializeObject(profilePictures);
        }

        public void SetDateModeEnabled(bool dateModeEnabled) {
            DateModeEnabled = dateModeEnabled;
        }
    }

    public class Image {
        public string Id { get; set; }
        public string Url { get; set; }
    }

}
