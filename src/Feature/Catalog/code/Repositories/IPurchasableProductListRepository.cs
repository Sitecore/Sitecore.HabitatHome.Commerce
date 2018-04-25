using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Feature.Catalog.Models;        

namespace Sitecore.Feature.Catalog.Repositories
{
    public interface IPurchasableProductListRepository
    {                                                                                        
        PurchasableProductListJsonResult GetPurchasableProductListJsonResult(IVisitorContext visitorContext, string currentItemId, string currentCatalogItemId, string searchKeyword, int? pageNumber, string facetValues, string sortField, int? pageSize, Sitecore.Commerce.XA.Foundation.Common.Constants.SortDirection? sortDirection);
    }
}
