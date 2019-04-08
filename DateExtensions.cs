using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalLib.Extensions
{
    public static class DateExtensions
    {
        public static DateTime GetByTimeZone(this DateTime dateTime, string timeZone = "GMT Standard Time")
        {
            TimeZoneInfo gmtTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, gmtTimeZone);
        }
    }
}