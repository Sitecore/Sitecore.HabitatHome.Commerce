using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Commerce.Plugin.Fulfillment;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Rules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Demo.HabitatHome.Conditions.Conditions
{
    [EntityIdentifier("CartItemHasPickUpCondition")]
    public class CartItemHasPickUpCondition : CartTargetItemId, ICartsCondition
    {
        public IRuleValue<string> StoreName { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            string targetItemId = this.TargetItemId.Yield(context);
            string storeName = this.StoreName.Yield(context);
            CommerceContext commerceContext = context.Fact<CommerceContext>((string)null);
            Cart cart = commerceContext != null ? commerceContext.GetObject<Cart>() : (Cart)null;
            if (cart == null || !cart.Lines.Any<CartLineComponent>() || (string.IsNullOrEmpty(targetItemId) || string.IsNullOrEmpty(storeName)))
                return false;
            //return this.MatchingLines(context).Where<CartLineComponent>(
            //    (Func<CartLineComponent, bool>)(
            //        l => l.HasComponent<CartProductComponent>()))
            //        .Any<CartLineComponent>(
            //            (Func<CartLineComponent, bool>)(
            //                l => (l.GetComponent<PhysicalFulfillmentComponent>().ShippingParty != null ?l.GetComponent<PhysicalFulfillmentComponent>().ShippingParty.AddressName.ToLower(): "") == storeName.ToLower()
            //                )
            //            );
            var cartLinesWithParty = cart.Lines.Where<CartLineComponent>(l => l.GetComponent<PhysicalFulfillmentComponent>().ShippingParty != null);

            if (cart.Lines.Where<CartLineComponent>((Func<CartLineComponent, bool>)(
                l => l.GetComponent<PhysicalFulfillmentComponent>().ShippingParty != null)).FirstOrDefault() != null)
            {
                var matchingParty = cart.Lines.Where(l => l.GetComponent<PhysicalFulfillmentComponent>().ShippingParty != null && l.GetComponent<PhysicalFulfillmentComponent>().ShippingParty.AddressName.ToLower() == storeName.ToLower());
                if (matchingParty != null)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }

    }
}
