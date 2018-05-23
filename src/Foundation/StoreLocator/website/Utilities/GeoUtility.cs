using Sitecore.Analytics;
using Sitecore.HabitatHome.Foundation.StoreLocator.Models;

namespace Sitecore.HabitatHome.Foundation.StoreLocator.Utilities
{
    public class GeoUtility
    {
        public static UserLocation GetUserLocation()
        {
            UserLocation ul = new UserLocation();
            if (Tracker.Enabled)
            {
                if (!Tracker.IsActive)
                {
                    Tracker.StartTracking();
                }

                var tracker = Sitecore.Analytics.Tracker.Current;
                var userData = tracker.Interaction;

                if (userData.HasGeoIpData)
                {

                    ul.Latitude = userData.GeoData.Latitude != null ? userData.GeoData.Latitude.ToString() : "33.7512130";
                    ul.Longitude = userData.GeoData.Longitude != null ? userData.GeoData.Longitude.ToString() : "-117.8387910";
                }
            }

            return ul;
        }
    }
}