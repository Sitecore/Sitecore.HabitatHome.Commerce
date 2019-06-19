using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.XA.Feature.Account.Models;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.XA.Foundation.SitecoreExtensions.Interfaces;

namespace Sitecore.HabitatHome.Feature.Orders.Models
{
    public class OrderLineVariantRenderingModel  : OrderLineRenderingModel
    {
        public OrderLineVariantRenderingModel(IStorefrontContext storefrontContext, ISearchManager searchManager, IContext context, IModelProvider modelProvider, IRendering rendering, ICurrencyFormatter currencyFormatter) 
            : base(storefrontContext, searchManager, context, modelProvider, rendering, currencyFormatter)
        { 
        }

        public string ProductVariantId { get; set; }

        public override void Initialize(CartLine orderLine, ShippingInfo shipping, Party party)
        {
            base.Initialize(orderLine, shipping, party);
            ProductVariantId = ((CommerceCartProduct)orderLine.Product).ProductVariantId;
        }
    }
}