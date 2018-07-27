using System.Collections.Generic;
using Sitecore.Commerce.XA.Feature.Catalog.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Repositories;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;

namespace Sitecore.HabitatHome.Feature.Catalog.Models
{
    public class PurchasableProductListJsonResult : ProductListJsonResult
    {                                                                                           
        public List<PurchasableProductSummaryViewModel> PurchasableChildProducts { get; protected set; }

        public PurchasableProductListJsonResult(IModelProvider modelProvider, IStorefrontContext storefrontContext, IContext context) 
            : base(modelProvider, storefrontContext, context)
        {
        }

        public new void Initialize(BaseCommerceModelRepository repository, List<ProductEntity> productEntityList, bool initializeAsMock = false, string searchKeyword = "")
        {
            base.Initialize(repository, productEntityList, initializeAsMock, searchKeyword);

            PurchasableChildProducts = new List<PurchasableProductSummaryViewModel>();

            foreach(var product in productEntityList)
            {
                PurchasableProductSummaryViewModel viewModel = this.ModelProvider.GetModel<PurchasableProductSummaryViewModel>();
                viewModel.Initialize(product, initializeAsMock);
                PurchasableChildProducts.Add(viewModel);
            }
        }
    }
}