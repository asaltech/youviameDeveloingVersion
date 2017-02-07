using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using youviame.Data.Repositories;
using youviame.Services;

namespace youviame.API.Controllers {
    [RoutePrefix("notification")]
    public class NotificationController : BaseApiController {
        private readonly INotificationService _notificationService;
        private readonly ILogRepository _logRepository;
        public NotificationController(INotificationService notificationService, ILogRepository logRepository)
        {
            _notificationService = notificationService;
            _logRepository = logRepository;
        }
        [HttpPost]
        [Route("pnmatch")]
        public HttpResponseMessage TestPNMatch([FromBody] SetNotificationRequest request)
        {
            try
            {
                _logRepository.InsertLog("request pnmatch method in  notification controller before send message");

                var applePushNotificationMessage = new ApplePushNotificationMessage(request.message, request.facebookId,request.type);

                _notificationService.Send(applePushNotificationMessage);
                _logRepository.InsertLog("request pnmatch method in  notification controller after send message");
                return Request.CreateResponse(HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                _logRepository.InsertLog("request pnmatch method in  notification controller failed" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not send notifications", e);

            }
            
        }

        [HttpGet]
        [Route("pndateperson")]
        public HttpResponseMessage TestPNDatePerson([FromBody] SetNotificationRequest request)
        {
            //var applePushNotificationMessage = new ApplePushNotificationMessage("HELLLUUUU111", "10157454213140604", MessageType.DatePerson);
            //_notificationService.Send(applePushNotificationMessage);
            try
            {
                _logRepository.InsertLog("request pndateperson method in  notification controller before send message");

                var applePushNotificationMessage = new ApplePushNotificationMessage(request.message, request.facebookId, request.type);

                _notificationService.Send(applePushNotificationMessage);
                _logRepository.InsertLog("request pndateperson method in  notification controller after send message");
                return Request.CreateResponse(HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                _logRepository.InsertLog("request pndateperson method in  notification controller failed" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not send notifications", e);

            }
        }


        [HttpGet]
        [Route("pnmessage")]
        public HttpResponseMessage TestPNMessage([FromBody] SetNotificationRequest request)
        {
            //var applePushNotificationMessage = new ApplePushNotificationMessage("API: Hi there!!!!", "10157454213140604", MessageType.NewMessage);
            //_notificationService.Send(applePushNotificationMessage);
            try
            {
                _logRepository.InsertLog("request pnmessage method in  notification controller before send message");

                var applePushNotificationMessage = new ApplePushNotificationMessage(request.message, request.facebookId, request.type);

                _notificationService.Send(applePushNotificationMessage);
                _logRepository.InsertLog("request pnmessage method in  notification controller after send message");
                return Request.CreateResponse(HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                _logRepository.InsertLog("request pnmessage method in  notification controller failed" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not send notifications", e);

            }
            
        }
    }

    public class SetNotificationRequest
    {
        public string message { get; set; }
        public string facebookId { get; set; }
        public MessageType type { get; set; }
     

    }
}