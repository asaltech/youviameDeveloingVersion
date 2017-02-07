using System;

namespace youviame.API.Controllers {
    public class UpdateProfileResponse {
        public Guid UserId { get; set; }
        public EditProfileModel NewValue { get; set; }
        public EditProfileModel OldValue { get; set; }

        public UpdateProfileResponse(Guid userId, EditProfileModel newValue, EditProfileModel oldValue) {
            UserId = userId;
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}