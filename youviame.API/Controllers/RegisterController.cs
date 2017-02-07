using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using log4net;
using youviame.Data.Context;
using youviame.Data.Enitities;
using youviame.Data.Repositories;

namespace youviame.API.Controllers {
    [RoutePrefix("register")]
    public class RegisterController : BaseApiController {

        private NotificationHubClient _hub;
      
      
        private  ILogRepository _logRepository;
        public RegisterController(ILogRepository logRepository)
        {
            _logRepository = logRepository;
            _logRepository.InsertLog("Post method in register controller triggered before _hub = Notifications.Instance.Hub");
            this._logRepository = logRepository;

            _hub = Notifications.Instance.Hub;

            _logRepository.InsertLog("Post method in register controller triggered hub is null" + (_hub == null));
           

        }

        public class DeviceRegistration {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        [Route("post")]
        [HttpPost]
        public async Task<string> Post(string handle = null) {

            _logRepository.InsertLog("Post method in register controller triggered");
            

            string newRegistrationId = null;
            try {
            if (handle != null) {
                handle = handle.ToUpper();

                var registrations = await _hub.GetRegistrationsByChannelAsync(handle, 100);
                _logRepository.InsertLog(" registraions is null " + (registrations == null));
               
            
                //_context.SaveChanges();
                foreach (var registration in registrations) {
                    if (newRegistrationId == null) {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else {
                        await _hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null) {
                newRegistrationId = await _hub.CreateRegistrationIdAsync();
            }
            _logRepository.InsertLog(" post in register success");
            
            }
            catch (Exception e) {
                _logRepository.InsertLog(" post in register failed" + e.Message);
                var exception = e;
                throw;
            }
            return newRegistrationId;
        }

        [Route("put")]
        [HttpPut]
        public async Task<HttpResponseMessage> Put([FromUri]string id, [FromBody] DeviceRegistration deviceUpdate, [FromUri]string facebookId) {
            _logRepository.InsertLog(" put in register triggered");
            RegistrationDescription registration = null;
            switch (deviceUpdate.Platform) {
                case "apns":
                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            registration.RegistrationId = id;
            if(deviceUpdate.Tags != null)
                registration.Tags = new HashSet<string>(deviceUpdate.Tags);
            else 
                registration.Tags = new HashSet<string>();
            registration.Tags.Add("facebookId:" + facebookId);

            try {
                await _hub.CreateOrUpdateRegistrationAsync(registration);
                _logRepository.InsertLog(" put in register success");
            }
            catch (MessagingException e) {
                _logRepository.InsertLog(" failed" + e.Message);
                ReturnGoneIfHubResponseIsGone(e);
            }


            return Request.CreateResponse(HttpStatusCode.OK, facebookId);
        }

        public async Task<HttpResponseMessage> Delete(string id) {
            await _hub.DeleteRegistrationAsync(id);
            _logRepository.InsertLog("delete in register triggered");
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        private static void ReturnGoneIfHubResponseIsGone(MessagingException e) {
            var webex = e.InnerException as WebException;
            if (webex.Status == WebExceptionStatus.ProtocolError) {
                var response = (HttpWebResponse) webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone) {
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
                }
            }
        }
    }
}