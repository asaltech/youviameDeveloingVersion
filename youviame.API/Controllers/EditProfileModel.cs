using System.Collections.Generic;
using youviame.Data.Enitities;

namespace youviame.API.Controllers {
    public class EditProfileModel {
        public string Location { get; set; }
        public string AboutMe { get; set; }
        public bool DateModeEnabled { get; set; }
        public List<Image> ProfilePictures { get; set; }
    }
}