using System;
using System.Collections.Generic;
using youviame.Data.Enitities;

namespace youviame.API.Controllers {
    public class UserViewModel {

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FacebookId { get; set; }
        public List<Image> ProfilePictures { get; set; }
        public string AboutMe { get; set; }
        public string Location { get; set; }
        public bool DateModeEnabled { get; set; }
    }
}