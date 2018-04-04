

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
        public IEnumerable<InventoryStore> GetNearestStores(UserLocation userLocation, string pid)
        {
            if (userLocation == null)
                throw new ArgumentNullException(nameof(userLocation));
            if(String.IsNullOrEmpty(pid))
                throw new ArgumentNullException(nameof(pid));
            List<InventoryStore> inventoryStores = new List<InventoryStore>();
            inventoryStores = nm.GetNearestStores(userLocation, pid).Take(2).ToList();
            if (inventoryStores.Count() > 0)
            {
                foreach (var store in inventoryStores)
                    store.InventoryAmount = nm.GetProductInventory(store.InventoryStoreId, pid);

                
            }
            return inventoryStores;
        }

        public IEnumerable<InventoryStore> GetStoresInventory(string pid)
        {            
            if (String.IsNullOrEmpty(pid))
                throw new ArgumentNullException(nameof(pid));

            IEnumerable<InventoryStore> inventoryStores = nm.GetSavedStoresInventory(pid);
            foreach (var store in inventoryStores)
                store.InventoryAmount = nm.GetProductInventory(store.InventoryStoreId, pid);
            return inventoryStores;
        }
    }
}