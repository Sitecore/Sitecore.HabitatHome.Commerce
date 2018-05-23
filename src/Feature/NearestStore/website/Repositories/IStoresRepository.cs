using System.Collections.Generic;
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.HabitatHome.Foundation.StoreLocator.Models;

namespace Sitecore.HabitatHome.Feature.NearestStore.Repositories
{
    public interface IStoresRepository
    {
        IEnumerable<InventoryStore> GetNearestStores(string pid);
        IEnumerable<InventoryStore> GetStoresInventory(string pid);
        CatalogItemRenderingModel GetNearestStoreRenderingModel(IVisitorContext visitorContext);
    }
}