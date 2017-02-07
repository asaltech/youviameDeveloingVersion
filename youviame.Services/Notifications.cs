using Microsoft.Azure.NotificationHubs;

namespace youviame.Services {
    public class Notifications {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications() {
            Hub =
                NotificationHubClient.CreateClientFromConnectionString(
                    "Endpoint=sb://youviame-dev-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=uyswlIFnNkHrEebXVYDpc+VLkV16EmXiF+OCaye/snY=",
                    "youviame-Dev-NotificationHub");
        }
    }
}