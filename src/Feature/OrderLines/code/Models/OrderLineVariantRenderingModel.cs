using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Commerce.XA.Feature.Account.Models;
using Sitecore.Commerce.Engine.Connect.Entities;

namespace Sitecore.Feature.OrderLines.Models
{
    public class OrderLineVariantRenderingModel  : OrderLineRenderingModel
    {
        public OrderLineVariantRenderingModel(IStorefrontContext storefrontContext, ISearchManager searchManager) : base(storefrontContext, searchManager)
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