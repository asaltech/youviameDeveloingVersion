using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using youviame.Data.Enitities;

namespace youviame.API.Controllers {
    public static class MatchExtensions {
        public static MatchViewModel ToViewModel(this Match match) {
            return new MatchViewModel {
                Id = match.Id,
                DatePerson1 = match.DatePerson1.ToViewModel(),
                DatePerson2 = match.DatePerson2.ToViewModel(),
                MatchMaker = match.MatchMaker.ToViewModel(),
                MatchStatus =  match.MatchStatus,
                MatchMakerStatus = match.MatchMakerStatus,
                DatePerson1Status = match.DatePerson1Status,
                DatePerson2Status = match.DatePerson2Status,
                DatePerson1Dates = !string.IsNullOrEmpty(match.DatePerson1Dates) ?  JsonConvert.DeserializeObject<List<double>>(match.DatePerson1Dates) : new List<double>(),
                DatePerson2Dates = !string.IsNullOrEmpty(match.DatePerson2Dates) ?  JsonConvert.DeserializeObject<List<double>>(match.DatePerson2Dates) : new List<double>(),
                Date = match.Date,
                MatchMakerMessage = match.MatchMakerMessage,
                Place = JsonConvert.DeserializeObject<PlaceViewModel>(match.Place),
                DatePerson1DateSetCount = match.DatePerson1DateSetCount,
                DatePerson2DateSetCount = match.DatePerson2DateSetCount

            };
        }
    }
}