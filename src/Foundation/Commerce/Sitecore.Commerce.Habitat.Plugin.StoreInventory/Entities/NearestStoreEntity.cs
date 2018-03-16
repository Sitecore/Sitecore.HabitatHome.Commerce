using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Plugin.Demo.HabitatHome.StoreInventorySet.Components;

namespace Plugin.Demo.HabitatHome.StoreInventorySet.Entities
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
