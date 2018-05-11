namespace Sitecore.Feature.NearestStore.Repositories
{
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

    public class StoresRepository : BaseCatalogRepository, IStoresRepository
    {
        private readonly NearestStoreManager _nearestStoreManager;

        public StoresRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, ICatalogUrlManager catalogUrlManager) : base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager)
        {
            _nearestStoreManager = new NearestStoreManager();
        }

        public virtual CatalogItemRenderingModel GetNearestStoreRenderingModel(IVisitorContext visitorContext)
        {
            return this.GetProduct(visitorContext);
        }

        public IEnumerable<InventoryStore> GetNearestStores(string pid)
        {
            if (string.IsNullOrEmpty(pid))
            {
                throw new ArgumentNullException(nameof(pid));
            }

            List<InventoryStore> inventoryStores = _nearestStoreManager.GetNearestStores().ToList();

            if (inventoryStores.Any())
            {
                foreach (var store in inventoryStores)
                {
                    store.InventoryAmount = _nearestStoreManager.GetProductInventory(store.InventoryStoreId, pid);
                }                
            }

            return inventoryStores;
        }

        public IEnumerable<InventoryStore> GetStoresInventory(string pid)
        {
            if (string.IsNullOrEmpty(pid))
            {
                throw new ArgumentNullException(nameof(pid));
            }

            List<InventoryStore> inventoryStores = _nearestStoreManager.GetSavedStores().ToList();

            foreach (var store in inventoryStores)
            {
                store.InventoryAmount = _nearestStoreManager.GetProductInventory(store.InventoryStoreId, pid);
            }

            return inventoryStores;
        }
    }
}