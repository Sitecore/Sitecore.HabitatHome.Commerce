using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Components
{
    public class StoreDetailsComponent : Component
    {
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string ZipCode { get; set; }
        public string CountryCode { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
    }
}
