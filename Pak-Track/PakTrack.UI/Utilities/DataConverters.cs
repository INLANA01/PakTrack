using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PakTrack.UI.Utilities
{
    public static class DataConverters
    {
        private static readonly DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static string TimeStampConverter(long timestamp)
        {
            
            return new DateTime(timestamp).ToString("MMM dd, yyyy hh:mm:ss tt");
            
        }

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

    }
}