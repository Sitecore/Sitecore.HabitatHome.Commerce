using Sitecore.Analytics;
using Sitecore.Feature.NearestStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.NearestStore.Utilities
{
    public static class GeoUtil
    {
        public static UserLocation GetUserLocation()
        {
            UserLocation ul = new UserLocation();
            try
            {
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

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ul;
        }
    }
}