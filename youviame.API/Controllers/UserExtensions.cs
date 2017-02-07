using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using youviame.Data.Enitities;

namespace youviame.API.Controllers {
    public static class UserExtensions {
        public static UserViewModel ToViewModel(this User user) {
            var viewModel = new UserViewModel {
                FirstName = user.FirstName,
                LastName = user.LastName,
                AboutMe = user.AboutMe,
                FacebookId = user.FacebookId,
                Id = user.Id,
                Location = user.Location,
                ProfilePictures = JsonConvert.DeserializeObject<List<Image>>(user.ProfilePictures),
                DateModeEnabled = user.DateModeEnabled
            };
            return viewModel;
        }
    }

    public static class ChatMessageRequestExtensions {
        public static ChatMessage ToChatMessage(this ChatMessageRequest message) {
            return new ChatMessage {
                DateTime = message.DateTime,
                MatchId = Guid.Parse(message.MatchId),
                UserId = Guid.Parse(message.UserId),
                Message = message.Message
            };
        }
    }
}