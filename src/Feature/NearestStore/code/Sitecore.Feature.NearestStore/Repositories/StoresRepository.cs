

namespace Sitecore.Feature.NearestStore.Repositories
{
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Feature.NearestStore.Managers;
    using Sitecore.Feature.NearestStore.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    //[Service(typeof(IStoresRepository))]  TODO: REFERENCE FOUNDATION.DEPENDENCYINJECTION
    public class StoresRepository : IStoresRepository
    {
        private NearestStoreManager nm;

        public StoresRepository()
        {
            nm = new NearestStoreManager();
        }
        public IEnumerable<InventoryStore> GetNearestStores(UserLocation userLocation)
        {
            NearestStoreManager nm = new NearestStoreManager();
            if (userLocation == null)
                throw new ArgumentNullException(nameof(userLocation));

            IEnumerable<InventoryStore> inventoryStores = nm.GetNearestStores(userLocation);
            
            return inventoryStores;
        }
    }
}