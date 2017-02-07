using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using youviame.Data.Context;
using youviame.Data.Enitities;
using youviame.Data.Repositories;

namespace youviame.API.Controllers
{
    [Authorize]
    [RoutePrefix("Report")]
    public class ReportController : ApiController
    {
        private readonly ILogRepository _logRepository;
        private readonly IReportRepository _ReportRepository;

        public ReportController(IReportRepository reportRepository, ILogRepository logRepository)
          {
              _logRepository = logRepository;
              _ReportRepository = reportRepository;

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
                return Request.CreateResponse(HttpStatusCode.Accepted);  

            }
            catch (Exception e)
            {
                _logRepository.InsertLog("Report insert failed");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not set report details", e);
            }
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
