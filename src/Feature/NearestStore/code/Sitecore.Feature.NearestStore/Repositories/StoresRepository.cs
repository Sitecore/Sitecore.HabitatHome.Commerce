

namespace Sitecore.Feature.NearestStore.Repositories
{
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Feature.NearestStore.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    //[Service(typeof(IStoresRepository))]  TODO: REFERENCE FOUNDATION.DEPENDENCYINJECTION
    public class StoresRepository : IStoresRepository
    {
        public IEnumerable<InventorySet> GetNearestStores(UserLocation userLocation)
        {
            if (userLocation == null)
                throw new ArgumentNullException(nameof(userLocation));

            List<InventorySet> inventorySet = new List<InventorySet>();
            return inventorySet;
        }
    }
}