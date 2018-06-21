using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Foundation.Catalog.Managers
{
    public interface IProductRelatedContentManager
    {
        IEnumerable<Item> GetAssociatedProducts(string productID, string associatedProductsField);
        IEnumerable<Item> GetProductContent(string productID);
    }
}