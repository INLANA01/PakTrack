using System;
using System.Data;
using LiteDB;

namespace PakTrack.DTO
{
    public abstract class BaseDTO
    {
        private long _timestamp;
        public ObjectId Id { get; set; }

        public long Timestamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                SetDateTime(value);
            }
        }



        public bool IsAboveThreshold { get; set; }

        public DateTime DateTime { get; set; }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return epoch.AddMilliseconds(unixTime);
        }

        /// <summary>
        /// Set the dateime from the unix timestamp store in the database, this is usefull for plotting chart.
        /// </summary>
        /// <param name="timestamp">Unix timestamp (milisecond)</param>
        private void SetDateTime(long timestamp)
        {
            DateTime = new DateTime(timestamp);
        }
    }
}