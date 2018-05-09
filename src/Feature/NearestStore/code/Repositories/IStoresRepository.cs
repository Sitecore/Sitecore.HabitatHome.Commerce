namespace Sitecore.Feature.NearestStore.Repositories
{
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Commerce.XA.Feature.Catalog.Models;
    using Sitecore.Commerce.XA.Foundation.Connect;
    using Sitecore.Foundation.Commerce.StoreLocator.Models;
    using System.Collections.Generic;    

    public interface IStoresRepository
    {
        IEnumerable<InventoryStore> GetNearestStores(string pid);
        IEnumerable<InventoryStore> GetStoresInventory(string pid);
        CatalogItemRenderingModel GetNearestStoreRenderingModel(IVisitorContext visitorContext);
    }
}