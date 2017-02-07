using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace youviame.Data.Enitities {
    public class Match : BaseEntity {
        public User MatchMaker { get; set; }
        public User DatePerson1 { get; set; }
        public User DatePerson2 { get; set; }   
        public MatchMakerStatus MatchMakerStatus { get; set; }
        public DatePersonStatus DatePerson1Status { get; set; }
        public DatePersonStatus DatePerson2Status { get; set; }

        public MatchStatus MatchStatus { get; set; }

        public string DatePerson1Dates { get; set; }
        public string DatePerson2Dates { get; set; }
        public int DatePerson1DateSetCount { get; set; }
        public int DatePerson2DateSetCount { get; set; }
        public double Date { get; set; }
        public string MatchMakerMessage { get; set; }
        public string Place { get; set; }

        public Match() {
            DatePerson1Dates = string.Empty;
            DatePerson2Dates = string.Empty;
            Date = 0;
            MatchMakerMessage = string.Empty;
            Place = string.Empty;
            DatePerson1DateSetCount = 0;
            DatePerson2DateSetCount = 0;
        }

        public void SetDatePersonDates(List<double> dates, Guid userId) {
            if (DatePerson1.Id == userId) {
                DatePerson1Dates =  JsonConvert.SerializeObject(dates);
                DatePerson1Status = DatePersonStatus.TimeChosen;
                DatePerson1DateSetCount++;
            } else if (DatePerson2.Id == userId) {
                DatePerson2Dates = JsonConvert.SerializeObject(dates);
                DatePerson2Status = DatePersonStatus.TimeChosen;
                DatePerson2DateSetCount++;
            }
            if (DatePerson1Status == DatePersonStatus.TimeChosen && DatePerson2Status == DatePersonStatus.TimeChosen) {
                var datePerson1Dates = JsonConvert.DeserializeObject<List<double>>(DatePerson1Dates);
                var datePerson2Dates = JsonConvert.DeserializeObject<List<double>>(DatePerson2Dates);
                if (DatePerson1DateSetCount == DatePerson2DateSetCount) {
                    MatchStatus = datePerson1Dates.Intersect(datePerson2Dates).Any() ? MatchStatus.NeedsPlanning : MatchStatus.DatesNotMatching;
                }
                else {
                    MatchStatus = MatchStatus.DatesNotMatching;
                }
            }

        }

        public bool IsActive() {
            var matchDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(this.Date));
            var now = DateTime.UtcNow;
            var difference = now - matchDate;
            return difference.TotalHours <= 48;
        }
        public void SetDateDetails(double date, string message, string placeName, string placeDetails) {
            Date = date;
            MatchStatus = MatchStatus.Planned;
            MatchMakerStatus = MatchMakerStatus.DatePlanned;
            MatchMakerMessage = message;
            var place = new Place {
                Name = placeName,
                Detail = placeDetails
            };
            Place = JsonConvert.SerializeObject(place);
        }

        public void SetDatePersonStatus(Guid userId, DatePersonStatus status) {
            MatchStatus = MatchStatus.Waiting;
            MatchMakerStatus = MatchMakerStatus.Sent;
            if (DatePerson1.Id == userId) {
                DatePerson1Status = status;
            }
            else if (DatePerson2.Id == userId) {
                DatePerson2Status = status;
            }

            if (DatePerson1Status == DatePersonStatus.Declined ||
                DatePerson2Status == DatePersonStatus.Declined) {
                MatchStatus = MatchStatus.UnMatched;
            }

            if (DatePerson1Status == DatePersonStatus.Accepted &&
                DatePerson2Status == DatePersonStatus.Accepted) {
                
                MatchMakerStatus = MatchMakerStatus.Matched;
                MatchStatus = MatchStatus.DateMissing;
            }
        }
    }

    public class Place {
        public string Name { get; set; }
        public string Detail { get; set; }

    }

    public enum DatePersonStatus {
        NoAnswer,
        Declined,
        Accepted,
        TimeChosen
    }

    public enum MatchMakerStatus {
        Sent,
        Matched,
        DatePlanned
    }

    public enum MatchStatus {
        Waiting,
        UnMatched,
        Matched,
        DateMissing,
        DatesNotMatching,
        NeedsPlanning,
        Planned
    }

}