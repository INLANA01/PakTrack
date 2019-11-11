namespace PakTrack.Models
{
    public interface ISensorConfigurationBase
    {
        double MaxThreshold { get; set; }
        double MinThreshold { get; set; }
        int TimePeriod { get; set; }
        int TimePeriodAfterThreshold { get; set; }
    }
}