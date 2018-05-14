namespace Sitecore.Feature.NearestStore.Repositories
{
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Commerce.XA.Feature.Catalog.Models;
    using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
    using Sitecore.Commerce.XA.Foundation.Common;
    using Sitecore.Commerce.XA.Foundation.Common.Models;
    using Sitecore.Commerce.XA.Foundation.Common.Search;
    using Sitecore.Commerce.XA.Foundation.Connect;
    using Sitecore.Commerce.XA.Foundation.Connect.Managers;
    using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
    using Sitecore.Feature.NearestStore.Managers;
    using Sitecore.Foundation.Commerce.StoreLocator.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class StoresRepository : BaseCatalogRepository, IStoresRepository
    {
        private NearestStoreManager nm;

        public StoresRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, ICatalogUrlManager catalogUrlManager) : base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager)
        {
            nm = new NearestStoreManager();
        }
        public virtual CatalogItemRenderingModel GetNearestStoreRenderingModel(IVisitorContext visitorContext)
        {
            return this.GetProduct(visitorContext);
        }
        public IEnumerable<InventoryStore> GetNearestStores(string pid)
        {           
            if(String.IsNullOrEmpty(pid))
                throw new ArgumentNullException(nameof(pid));
            List<InventoryStore> inventoryStores = new List<InventoryStore>();
            inventoryStores = nm.GetNearestStores().ToList();
            if (inventoryStores.Count() > 0)
            {
                foreach (var store in inventoryStores)
                    store.InventoryAmount = nm.GetProductInventory(store.InventoryStoreId, pid);                
            }
            return inventoryStores;        }

        public IEnumerable<InventoryStore> GetStoresInventory(string pid)
        {            
            if (String.IsNullOrEmpty(pid))
                throw new ArgumentNullException(nameof(pid));
            IEnumerable<InventoryStore> inventoryStores = nm.GetSavedStores();
            foreach (var store in inventoryStores)
                store.InventoryAmount = nm.GetProductInventory(store.InventoryStoreId, pid);
            return inventoryStores;
        }
    }
}