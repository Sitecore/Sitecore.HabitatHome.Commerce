namespace Sitecore.Feature.NearestStore.Repositories
{
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Feature.NearestStore.Models;
    using System.Collections.Generic;    

    public interface IStoresRepository
    {
        IEnumerable<InventoryStore> GetNearestStores(UserLocation userLocation);
    }
}