using System;
using Microsoft.Azure.NotificationHubs;
using youviame.Data.Enitities;
using youviame.Data.Context;
using youviame.Data.Repositories;

namespace youviame.API.Controllers {
    public class Notifications {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }
        public ILogRepository _logRepository;
        private Notifications(ILogRepository logRepository)
        {
            _logRepository = logRepository;
            _logRepository.InsertLog(" Inside Notifications Notifications()");            
            //Hub =
            //    NotificationHubClient.CreateClientFromConnectionString(
            //        "Endpoint=sb://youviame-dev-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=uyswlIFnNkHrEebXVYDpc+VLkV16EmXiF+OCaye/snY=",
            //        "youviame-Dev-NotificationHub");

            Hub =
                NotificationHubClient.CreateClientFromConnectionString(
                    "Endpoint=sb://youviame-dev-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=uyswlIFnNkHrEebXVYDpc+VLkV16EmXiF+OCaye/snY=",
                    "youviame-Dev-NotificationHub");

            //Endpoint=sb://youviame-dev-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1h00aZeIVL+fFQ8O0riySEexgHrCS6q024V4XkqW16w=
            _logRepository.InsertLog(" Inside Notifications Notifications(,  Hub = null: " + (Hub == null)); 
            

        }

        private Notifications()
        {
            //_logRepository.InsertLog(" Inside Notifications Notifications()");
            //Hub =
            //    NotificationHubClient.CreateClientFromConnectionString(
            //        "Endpoint=sb://youviame-dev-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=uyswlIFnNkHrEebXVYDpc+VLkV16EmXiF+OCaye/snY=",
            //        "youviame-Dev-NotificationHub");

            Hub =
                NotificationHubClient.CreateClientFromConnectionString(
                    "Endpoint=sb://youviame-dev-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=uyswlIFnNkHrEebXVYDpc+VLkV16EmXiF+OCaye/snY=",
                    "youviame-Dev-NotificationHub");

            //Endpoint=sb://youviame-dev-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1h00aZeIVL+fFQ8O0riySEexgHrCS6q024V4XkqW16w=
           // _logRepository.InsertLog(" Inside Notifications Notifications(,  Hub = null: " + (Hub == null));


        }
    }
}