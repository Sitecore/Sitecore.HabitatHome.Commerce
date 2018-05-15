using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Entities
{
    public class NearestStoreLocation : CommerceEntity
    {
        public double Distance { get; set; }
        public string InventoryStoreId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip  { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
