namespace Microsoft.Samples.DPE.BlobShare.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public static class DateTimeHelper
    {
        private static TimeZoneInfo localZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        public static DateTime ToLocalTime(DateTime datetime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(datetime, localZoneInfo);
        }

        public static DateTime FromLocalTime(DateTime datetime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(datetime, DateTimeKind.Unspecified), localZoneInfo);
        }
    }
}