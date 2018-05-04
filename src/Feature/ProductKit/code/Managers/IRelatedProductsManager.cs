using Sitecore.Commerce.XA.Feature.Catalog.Models.ProductLists;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.Diagnostics;
using Sitecore.Feature.ProductKit.Models.JsonResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.ProductKit.Managers
{
    public interface IRelatedProductsManager
    {
        IEnumerable<RelatedProductJsonResult> GetRelatedProducts(string productID);       
    }
}