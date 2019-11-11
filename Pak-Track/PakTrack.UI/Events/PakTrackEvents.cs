using PakTrack.Core;
using PakTrack.UI.Utilities;
using Prism.Events;

namespace PakTrack.UI.Events
{
    public class NavigationEvent: PubSubEvent<NavigationInformation>
    {
        
    }

    public class TruckAddEvent : PubSubEvent<NavigationInformation>
    {
        
    }

    public class RemoteNavigationEvent : PubSubEvent<string>
    {
        
    }

    public class StatusEvent : PubSubEvent<StatusInformation>
    {
        
    }

    public class BatteryStatus : PubSubEvent<string>
    {
        
    }

    public class CustomReportFilterEvent : PubSubEvent<FilterInfo>
    {
        
    }
}