using System.Threading.Tasks;

namespace youviame.Services {
    public interface INotificationService {
        Task Send(IPushNotificationMessage message);
    }

    public interface IPushNotificationMessage {
        string Pns { get; }
        string Message { get; }
        string ReceieverFacebookId { get; }
        MessageType MessageType { get; }
    }

    public class ApplePushNotificationMessage : IPushNotificationMessage {
        public ApplePushNotificationMessage(string message, string receiverFacebookId, MessageType type) {
            this.Pns = "apns";
            this.Message = message;
            this.ReceieverFacebookId = receiverFacebookId;
            this.MessageType = type;
        }

        public string Pns { get; set; }
        public string Message { get; set; }
        public string ReceieverFacebookId { get; set; }
        public MessageType MessageType { get; set; }
    }


    public enum MessageType {
        Unknown, 
        MatchMaker,
        DatePerson,
        NewMessage
    }
}