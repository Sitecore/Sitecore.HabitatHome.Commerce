using Sitecore.Commerce.XA.Feature.Cart.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;

namespace Sitecore.HabitatHome.Feature.Orders.Models.JsonResults
{
    public class OrderReviewDataJsonResult : ReviewDataJsonResult
    {
        public OrderReviewDataJsonResult(IStorefrontContext storefrontContext, IModelProvider modelProvider, IContext context)
            : base(storefrontContext, modelProvider, context)
        {
        }
        public BenefitsDataJsonResult BenefitsData { get; set; }
    }
}