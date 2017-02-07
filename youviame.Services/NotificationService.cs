using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace youviame.Services {
    public class NotificationService : INotificationService {
        public async Task Send(IPushNotificationMessage message) {
            var userTag = new string[2];
            userTag[0] = "facebookId:" + message.ReceieverFacebookId;
            userTag[1] = "from:api";
            var jsonSerializerSettings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            NotificationOutcome outcome = null;
            switch (message.Pns) {
                case "apns": {
                    var alert = JsonConvert.SerializeObject(new ApplePushMessage(message.Message, message.MessageType.ToString()), jsonSerializerSettings);// "{\"aps\":{\"alert\":\"" + message + "\"}}";

                    outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                    break;
                }
            }
        }

        internal class ApplePushMessage {
            public Aps Aps { get; set; }

            public ApplePushMessage(string message, string messageType) {
                Aps = new Aps {
                    Alert = message,
                    MessageType = messageType
                };
            }
        }

        internal class Aps {
            public string Alert { get; set; }
            public string MessageType { get; set; }
        }
    }

    
}