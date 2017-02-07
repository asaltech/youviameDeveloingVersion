using System;

namespace youviame.API.Controllers {
    public class UpdateProfileRequest {
        public Guid UserId { get; set; }
        public EditProfileModel Values { get; set; }
    }
}