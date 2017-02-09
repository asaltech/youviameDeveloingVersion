using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using youviame.Data.Context;
using youviame.Data.Enitities;
using youviame.Data.Repositories;
using System.Net.Mail;
using System.Text;

namespace youviame.API.Controllers
{
   // [Authorize]
    [RoutePrefix("Report")]
    public class ReportController : ApiController
    {
        private readonly ILogRepository _logRepository;
        private readonly IReportRepository _ReportRepository;
        private readonly IUserRepository _userRepository;
        public ReportController(IReportRepository reportRepository, ILogRepository logRepository, IUserRepository userRepository)
          {
              _logRepository = logRepository;
              _ReportRepository = reportRepository;
              _userRepository = userRepository;
          }

        [HttpPost]
        [Route("sendreport")]
        public HttpResponseMessage SetReportDetails([FromBody] SetReportDetailsRequest request)
        {
            _logRepository.InsertLog("Report SetReportDetails requested");
            try
            {
                Report entity = new Report();
                entity.ReportedUser = request.ReportedUser;
                entity.ReporterId = request.ReporterId;
                entity.ReportMessage = request.ReportMessage;  
                entity.ReportReason = request.ReportReason;

                _ReportRepository.Save(entity);
                _logRepository.InsertLog("Report insert successfully");
              sendEmail(request.ReportMessage,request.ReporterId,request.ReportedUser,request.ReportReason);
            // sendEmail(request.ReportMessage, new Guid("6177AA00-A785-4A2F-B88E-01F474ACD698"), new Guid("D6995DD7-00A7-485E-9F55-09359236EFF3"), request.ReportReason);
                return Request.CreateResponse(HttpStatusCode.Accepted);  

            }
            catch (Exception e)
            {
                _logRepository.InsertLog("Report insert failed" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not set report details", e);
            }
        }

        public void sendEmail(string message,Guid reporter,Guid reported,ReportReason reason)
        {
            _logRepository.InsertLog("inside send email");
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("abuseyouviame@gmail.com", "123456789youviame");
            var ReporterUser = _userRepository.Get(reporter);
            var Reported = _userRepository.Get(reported);

            /////
            Dictionary<ReportReason, string> ReportMessageType = new Dictionary<ReportReason, string>();
            ReportMessageType.Add(ReportReason.InappropiatePictures, "Olämpliga Bilder");
            ReportMessageType.Add(ReportReason.InappropriateBehaviour, "Olämpligt Offline beteende");
            ReportMessageType.Add(ReportReason.InappropriateMessages, "Olämpliga Meddelanden");
            ReportMessageType.Add(ReportReason.Other, "Annat");
            /////

            var body = ReporterUser.FirstName + " " + ReporterUser.LastName + " has reported user : " + Reported.FirstName + " " + Reported.LastName + " for this reason " + message;
            MailMessage mm = new MailMessage("abuseyouviame@gmail.com", "abuse@youviame.com ", "Report | " + (reason == null ? " " : ReportMessageType[reason]), body);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            try
            {
                _logRepository.InsertLog("before Sending Email success");
                client.Send(mm);
                _logRepository.InsertLog("after Sending Email success");
            }
            catch (Exception e) { _logRepository.InsertLog("an error occurred while sending email "+e.Message); }

        }
    }


    public class SetReportDetailsRequest
    {
        public Guid ReporterId { get; set; }
        public Guid ReportedUser  { get; set; }
        public string ReportMessage { get; set; }
        public ReportReason ReportReason { get; set; }

    }


  
}
