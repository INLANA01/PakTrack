using System;

namespace PakTrack.UI.Utilities
{
    public class AxisInfo
    {
        public double MinValue;
        public double MaxValue;
    }
    public class FilterInfo
    {
        public AxisInfo X;
        public AxisInfo Y;
        public AxisInfo Z;

        public DateTime Start;
        public DateTime End;

        public bool IsTimeStamp;
    }
}