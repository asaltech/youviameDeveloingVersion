using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using youviame.Data.Context;
using youviame.Data.Enitities;
using youviame.Data.Repositories;
using youviame.Services;

namespace youviame.API.Controllers {
    [RoutePrefix("match")]
    public class MatchController : BaseApiController {
        private readonly IMatchRepository _matchRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogRepository _logRepository;

        public MatchController(IMatchRepository matchRepository, IUserRepository userRepository, INotificationService notificationService, IChatRepository chatRepository, ILogRepository logRepository)
        {
            _matchRepository = matchRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _chatRepository = chatRepository;
            _logRepository = logRepository;
        }
               

        [HttpGet]
        [Route("actionable")]
        public HttpResponseMessage GetActionableMatches([FromUri]Guid userId) {

            _logRepository.InsertLog("GetActionableMatches in match controller requested");
            var userMatches = _matchRepository.GetAll().Where(x => x.DatePerson1.Id == userId || x.DatePerson2.Id == userId || x.MatchMaker.Id == userId);
            var matches = new List<Match>();
            foreach (var match in userMatches) {
                if (match.DatePerson1.Id == userId) {
                    if (match.DatePerson1Status == DatePersonStatus.NoAnswer &&
                        match.MatchStatus == MatchStatus.Waiting) {
                        matches.Add(match);
                        continue;
                    }
                    if (match.MatchStatus == MatchStatus.Planned) {
                        matches.Add(match);
                        continue;
                    }

                    if (match.DatePerson1Status == DatePersonStatus.Accepted &&
                        (match.DatePerson2Status == DatePersonStatus.Accepted ||
                         match.DatePerson2Status == DatePersonStatus.TimeChosen) && match.MatchStatus == MatchStatus.DateMissing) {
                        matches.Add(match);
                        continue;
                    }

                    if (match.DatePerson1DateSetCount <= match.DatePerson2DateSetCount &&
                        match.MatchStatus == MatchStatus.DatesNotMatching) {
                        matches.Add(match);
                        continue;
                    }
                }
                else if (match.DatePerson2.Id == userId) {
                    if (match.DatePerson2Status == DatePersonStatus.NoAnswer &&
                        match.MatchStatus == MatchStatus.Waiting) {
                        matches.Add(match);
                        continue;
                    }
                    if (match.MatchStatus == MatchStatus.Planned) {
                        matches.Add(match);
                        continue;
                    }
                    if (match.DatePerson2Status == DatePersonStatus.Accepted &&
                    (match.DatePerson1Status == DatePersonStatus.Accepted ||
                     match.DatePerson1Status == DatePersonStatus.TimeChosen) && match.MatchStatus == MatchStatus.DateMissing) {
                        matches.Add(match);
                        continue;
                    }

                    if (match.DatePerson2DateSetCount <= match.DatePerson1DateSetCount &&
                     match.MatchStatus == MatchStatus.DatesNotMatching) {
                        matches.Add(match);
                        continue;
                    }

                }
                else if (match.MatchMaker.Id == userId) {
                    if (match.MatchStatus == MatchStatus.NeedsPlanning) {
                        matches.Add(match);
                        continue;
                    }
                }
            }
            _logRepository.InsertLog("GetActionableMatches in match controller send response");
            return Request.CreateResponse(HttpStatusCode.OK, matches.Select(x => x.ToViewModel()));
        }

        [HttpPost]
        [Route("setdatedetails")]
        public HttpResponseMessage SetDateDetails([FromBody] SetDateDetailsRequest request) {

            

            _logRepository.InsertLog("setDateDetails is requested in match controller");

            var match = _matchRepository.Get(request.MatchId);
            match.SetDateDetails(request.Date, request.MatchMakerMessage, request.Place.Name, request.Place.Detail);
            try {
                _matchRepository.Update(match);
                match = _matchRepository.Get(request.MatchId);
                if (match.MatchMakerStatus == MatchMakerStatus.DatePlanned)
                {
                    var message = match.MatchMaker.FirstName + " " + match.MatchMaker.LastName + "\n bestämmer att du och" + match.DatePerson2.FirstName + " " + match.DatePerson2.LastName + "\nska gå till";
                    var applePushNotificationMessage = new ApplePushNotificationMessage(message, match.DatePerson1.FacebookId, MessageType.MatchMaker);
                    _notificationService.Send(applePushNotificationMessage);
                    _logRepository.InsertLog("send notification success to dateperson1 " + match.DatePerson1.FirstName);

                    message = match.MatchMaker.FirstName + " " + match.MatchMaker.LastName + "\n bestämmer att du och" + match.DatePerson1.FirstName + " " + match.DatePerson1.LastName + "\nska gå till";
                   applePushNotificationMessage = new ApplePushNotificationMessage(message, match.DatePerson2.FacebookId, MessageType.MatchMaker);
                    _notificationService.Send(applePushNotificationMessage);
                    _logRepository.InsertLog("send notification success to dateperson2 "+match.DatePerson2.FirstName);


                }
                _logRepository.InsertLog("setDateDetails is updated successfully in match controller");
                return Request.CreateResponse(HttpStatusCode.Accepted, match.ToViewModel());

            }
            catch (Exception e) {
                _logRepository.InsertLog("setDateDetails is failed in match controller" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not set date details", e);
            }
        }

        [HttpPost]
        [Route("setdates")]
        public HttpResponseMessage SetDates([FromBody] SetDatesRequest request) {
            
            _logRepository.InsertLog("setdates method in match controller requested");

            var match = _matchRepository.Get(request.MatchId);
            match.SetDatePersonDates(request.Dates, request.UserId);
            try {
                _matchRepository.Update(match);
                match = _matchRepository.Get(request.MatchId);
                // send notification to matchmaker when set dates
                if (match.DatePerson1Status == DatePersonStatus.TimeChosen && match.DatePerson2Status == DatePersonStatus.TimeChosen)
                {
                    var message = match.DatePerson1.FirstName + " " + match.DatePerson1.LastName + " & " + match.DatePerson2.FirstName + " " + match.DatePerson2.LastName + "\nhar matchande tider." + "\n Dags för dig att planera deras första dejt!";
                    var applePushNotificationMessage = new ApplePushNotificationMessage(message, match.MatchMaker.FacebookId, MessageType.DatePerson);
                    _notificationService.Send(applePushNotificationMessage);
                    _logRepository.InsertLog("send notification success to matchmaker "+match.MatchMaker.FirstName);
                  

                }



                //
                _logRepository.InsertLog("setdates method in match controller success");
                return Request.CreateResponse(HttpStatusCode.Accepted, match.ToViewModel());
            }
            catch (Exception e) {
                _logRepository.InsertLog("setdates method in match controller failed" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not set dates", e);
            }
        }

        [HttpPost]
        [Route("setstatus")]
        public HttpResponseMessage SetStatus([FromBody] SetMatchDatePersonStatusRequest request) {

            _logRepository.InsertLog("setstatus method in match controller requested");
            var match = _matchRepository.Get(request.MatchId);
            match.SetDatePersonStatus(request.UserId, request.Status);
            try {
                _matchRepository.Update(match);
                _logRepository.InsertLog("setstatus method in match controller updated success");
                 // check for send notification depend on status
                match = _matchRepository.Get(request.MatchId);
                if (match.DatePerson1Status == DatePersonStatus.Accepted && match.DatePerson2Status == DatePersonStatus.Accepted)
                {
                    string message = match.DatePerson2.FirstName + " " + match.DatePerson2.LastName + " \n vill träffa dig med." + "\n Dags att bestämma när ni kan ses"+"\ninom de närmaste 7 dagarna!";
                    var applePushNotificationMessage = new ApplePushNotificationMessage(message, match.DatePerson1.FacebookId, MessageType.MatchMaker);
                    _notificationService.Send(applePushNotificationMessage);
                    _logRepository.InsertLog("send notification success to person1 " + match.DatePerson1.FirstName);

                    message = match.DatePerson1.FirstName + " " + match.DatePerson1.LastName + " \n vill träffa dig med." + "\n Dags att bestämma när ni kan ses" + "\ninom de närmaste 7 dagarna!";
                    applePushNotificationMessage = new ApplePushNotificationMessage(message, match.DatePerson2.FacebookId, MessageType.MatchMaker);
                    _notificationService.Send(applePushNotificationMessage);
                    _logRepository.InsertLog("send notification success to person2 " + match.DatePerson2.FirstName);

                }
            


                //
                return Request.CreateResponse(HttpStatusCode.Accepted, match.ToViewModel());
            }
            catch (Exception e) {
                _logRepository.InsertLog("setstatus method in match controller updated failed" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not update match", e);
            }
        }

        [HttpGet]
        [Route("my")]
        public HttpResponseMessage GetMyMatches([FromUri] string userId, [FromUri] DatePersonStatus status = DatePersonStatus.NoAnswer) {
            _logRepository.InsertLog("GetMyMatches method in match controller requested");
            var guid = Guid.Parse(userId);
            var myMatches = _matchRepository.GetAll().Where(x => x.DatePerson1.Id == guid || x.DatePerson2.Id == guid);
            var myMatchesWithStatus = myMatches.Where(x => x.DatePerson1.Id == guid && x.DatePerson1Status == status ||
                                                  x.DatePerson2.Id == guid && x.DatePerson2Status == status).Select(x => x.ToViewModel());
            return Request.CreateResponse(HttpStatusCode.OK, myMatchesWithStatus);
        }

        [HttpGet]
        [Route("mymatched")]
        public HttpResponseMessage GetMyPlannedMatches([FromUri] string userId) {

            _logRepository.InsertLog("GetMyPlannedMatches method in match controller requested");
            var guid = Guid.Parse(userId);
            var myMatches = _matchRepository.GetAll().Where(x => (x.DatePerson1.Id == guid || x.DatePerson2.Id == guid));
            var myPlannedMatches = myMatches.Where(
                x => x.MatchStatus == MatchStatus.Planned)
                .Select(x => x.ToViewModel());
            return Request.CreateResponse(HttpStatusCode.OK, myPlannedMatches);
        }

        [HttpGet]
        [Route("active")]
        public HttpResponseMessage MyActiveDates([FromUri] string userId) {
            _logRepository.InsertLog("MyActiveDates method in match controller requested");
            var guid = Guid.Parse(userId);
            var myPlannedDates = _matchRepository.GetAll()
                .Where(x => x.DatePerson1.Id == guid || x.DatePerson2.Id == guid)
                .Where(x => x.MatchStatus == MatchStatus.Planned)
                .Where(x => x.IsActive())
                .Select(x => x.ToViewModel());
            return Request.CreateResponse(HttpStatusCode.OK, myPlannedDates);
        }

        [HttpGet]
        [Route("user")]
        public HttpResponseMessage MyMatches([FromUri] string userId) {

            _logRepository.InsertLog("MyMatches method in match controller requested");
            var guid = Guid.Parse(userId);
            var myMatches = _matchRepository.GetAll().Where(x => x.DatePerson1.Id == guid || x.DatePerson2.Id == guid);
            var myViewModels = myMatches.Where(
                x =>
                    x.DatePerson1Status != DatePersonStatus.Declined &&
                    x.DatePerson2Status != DatePersonStatus.Declined).Select(x => x.ToViewModel());
            return Request.CreateResponse(HttpStatusCode.OK, myViewModels);
        }

        [HttpGet]
        [Route("matchmaker")]
        public HttpResponseMessage GetMyMatches([FromUri] string matchmakerId) {

            _logRepository.InsertLog("GetMyMatches method in match controller requested");
            var guid = Guid.Parse(matchmakerId);
            var matches = _matchRepository.GetAll().Where(x => x.MatchMaker.Id == guid).Select(x => x.ToViewModel());
            matches =
                matches.Where(
                    x =>
                        (x.MatchStatus == MatchStatus.Waiting && x.MatchMakerStatus == MatchMakerStatus.Sent) ||
                        (x.MatchStatus == MatchStatus.DateMissing && x.MatchMakerStatus == MatchMakerStatus.Matched));
            return Request.CreateResponse(HttpStatusCode.OK, matches);
        }

        [HttpGet]
        [Route("chat")]
        public HttpResponseMessage GetChat([FromUri] string matchId) {
            var guid = Guid.Parse(matchId);
            var chatMessages = _chatRepository.Get(guid);
            return Request.CreateResponse(HttpStatusCode.OK, chatMessages);
        }

        [HttpPost]
        [Route("chat")]
        public async Task<HttpResponseMessage> SendChat([FromBody] ChatMessageRequest message) {
            var chatMessage = message.ToChatMessage();
            await _chatRepository.SaveAsync(chatMessage);
            return Request.CreateResponse(HttpStatusCode.Created, chatMessage);
        }

        [HttpPost]
        [Route("send")]
        public HttpResponseMessage SendMatches([FromBody] SendMatchesRequest request) {

            _logRepository.InsertLog("sendmatch method in match controller requested");
            var validMatches = new List<Data.Enitities.Match>();
            var invalidMatches = new List<InvalidMatch>();
            var matchMaker = _userRepository.Get(request.MatchMaker.FacebookId);
            foreach (var match in request.Matches) {
                var user1 = _userRepository.Get(match.DatePerson1.FacebookId);
                var user2 = _userRepository.Get(match.DatePerson2.FacebookId);
                if (user1 == null || user2 == null) {
                    var invalidMatch = new InvalidMatch(match);
                    if (user1 == null)
                       // invalidMatch.AddErrorMessage($"{match.DatePerson1.FirstName} has not yet registered in the app");
                        invalidMatch.AddErrorMessage(String.Format("{0} has not yet registered in the app",match.DatePerson1.FirstName));
                    if (user2 == null)
                        //invalidMatch.AddErrorMessage($"{match.DatePerson2.FirstName} has not yet registered in the app");
                        invalidMatch.AddErrorMessage(String.Format("{0} has not yet registered in the app", match.DatePerson2.FirstName));
                    invalidMatches.Add(invalidMatch);
                    continue;
                }
                var existingMatch = _matchRepository.Get(user1.Id, user2.Id);
                if (existingMatch != null) {
                    var invalidMatch = new InvalidMatch(match);
                    invalidMatch.AddErrorMessage("Match already exists");
                    invalidMatches.Add(invalidMatch);
                    continue;
                }
                var newPossibleMatch = new Data.Enitities.Match {
                    DatePerson1 = user1,
                    DatePerson2 = user2,
                    MatchMaker = matchMaker,
                    MatchStatus = MatchStatus.Waiting,
                    MatchMakerStatus = Data.Enitities.MatchMakerStatus.Sent,
                    DatePerson1Status = Data.Enitities.DatePersonStatus.NoAnswer,
                    DatePerson2Status = Data.Enitities.DatePersonStatus.NoAnswer,

                };
                validMatches.Add(newPossibleMatch);
            }
            try {
                _matchRepository.Save(validMatches);
                _logRepository.InsertLog("sendmatch method in match controller success");
                _logRepository.InsertLog("send notification");
                foreach (var item in validMatches)
                {   
                    string message="Du har blivit matchad med: \n" +item.DatePerson2.FirstName + " " + item.DatePerson2.LastName + " \n" + item.MatchMaker.FirstName + " " + item.MatchMaker.LastName + " tycker ni ska träffas.";
                    var applePushNotificationMessage = new ApplePushNotificationMessage(message , item.DatePerson1.FacebookId, MessageType.MatchMaker);
                    _notificationService.Send(applePushNotificationMessage);
                    _logRepository.InsertLog("send notification success for person1 " + item.DatePerson1.FirstName);
                    message = "Du har blivit matchad med: \n" + item.DatePerson1.FirstName + " " + item.DatePerson1.LastName + " \n" + item.MatchMaker.FirstName + " " + item.MatchMaker.LastName + " tycker ni ska träffas.";
                     applePushNotificationMessage = new ApplePushNotificationMessage(message, item.DatePerson2.FacebookId, MessageType.MatchMaker);
                    _notificationService.Send(applePushNotificationMessage);
                    _logRepository.InsertLog("send notification success person2 "+item.DatePerson2.FirstName);
                }
            }
            catch (Exception e) {
                _logRepository.InsertLog("sendmatch method in match controller failed" + e.Message);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Could not save matches", e);
            }

            var response = new SendMatchesResponse {
                CreatedMatches = validMatches.Select(x => x.ToViewModel()),
                InvalidMatches = invalidMatches
            };

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        [Route]
        public HttpResponseMessage Get() {
            var users = _userRepository.GetAll();
            var matches = _matchRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.Accepted, users);
        }
    }


    public class ChatMessageRequest {
        public string UserId { get; set; }
        public string MatchId { get; set; }
        public double DateTime { get; set; }
        public string Message { get; set; }
    }
    public class SetDateDetailsRequest {
        public Guid MatchId { get; set; }
        public PlaceViewModel Place { get; set; }
        public string MatchMakerMessage { get; set; }
        public double Date { get; set; }

    }

    public class SetDatesRequest {
        public Guid UserId { get; set; }
        public Guid MatchId { get; set; }
        public List<double> Dates { get; set; }

    }

    public class SetMatchDatePersonStatusRequest {
        public Guid UserId { get; set; }
        public Guid MatchId { get; set; }
        public DatePersonStatus Status { get; set; }

    }

    public class InvalidMatch {
        public SendMatch Match { get; set; }
        public List<string> ErrorMessages { get; set; }

        public InvalidMatch(SendMatch match) {
            Match = match;
            ErrorMessages = new List<string>();
        }

        public void AddErrorMessage(string errorMessage) {
            ErrorMessages.Add(errorMessage);
        }
    }
    public class SendMatchesResponse {
        public IEnumerable<MatchViewModel> CreatedMatches { get; set; }
        public IEnumerable<InvalidMatch> InvalidMatches { get; set; }
    }

    public class SendMatchesRequest {
        public UserViewModel MatchMaker { get; set; }
        public IEnumerable<SendMatch> Matches { get; set; }
    }

    public class SendMatch {
        public UserViewModel DatePerson1 { get; set; }
        public UserViewModel DatePerson2 { get; set; }
    }

    public class MatchViewModel {
        public Guid Id { get; set; }
        public UserViewModel MatchMaker { get; set; }
        public UserViewModel DatePerson1 { get; set; }
        public UserViewModel DatePerson2 { get; set; }
        public MatchStatus MatchStatus { get; set; }
        public MatchMakerStatus MatchMakerStatus { get; set; }
        public DatePersonStatus DatePerson1Status { get; set; }
        public DatePersonStatus DatePerson2Status { get; set; }
        public List<double> DatePerson1Dates { get; set; }
        public List<double> DatePerson2Dates { get; set; }

        public string MatchMakerMessage { get; set; }
        public double Date { get; set; }
        public PlaceViewModel Place { get; set; }
        public int DatePerson1DateSetCount { get; set; }
        public int DatePerson2DateSetCount { get; set; }
    }

    public class ActionableMatchViewModel {
        public MatchViewModel Match { get; set; }
        public MatchType Type { get; set; }
    }

    public enum MatchType {
        DatePersonRespondTo = 0,
        DatePersonChooseTime = 1,
        DatePersonItsAMatch = 2,
        MatchMakerPlan = 3
    }


    public class PlaceViewModel {
        public string Name { get; set; }
        public string Detail { get; set; }
    }
}
