using LiteDB;

namespace PakTrack.Core
{
    /// <summary>
    /// This model it's used to transport information related to truck, package and event ID
    /// </summary>
    public class NavigationInformation
    {
        public string PackageId { get; set; }
        public string TruckId { get; set; }
        public ObjectId EventId { get; set; }
    }
}