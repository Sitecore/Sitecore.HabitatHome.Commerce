using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;

namespace Sitecore.HabitatHome.Feature.Orders.Models
{
    public class OrderLineVariantRenderingModel  : Commerce.XA.Foundation.CommerceEngine.Models.OrderLineRenderingModel
    {
        public OrderLineVariantRenderingModel(IStorefrontContext storefrontContext, ISearchManager searchManager, IContext context) 
            : base(storefrontContext, searchManager, context)
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