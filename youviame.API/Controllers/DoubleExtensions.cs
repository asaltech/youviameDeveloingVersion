using System;

namespace youviame.API.Controllers {
    public static class DoubleExtensions {
        public static DateTime? ToDate(this double? unixTimeStamp) {
            if (unixTimeStamp == null)
                return null;
            else { 
                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return dateTime.AddSeconds((double)unixTimeStamp );
            }
        }

    }
}