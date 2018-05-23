using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Rules;

namespace Sitecore.HabitatHome.Foundation.Rules.Engine.Conditions
{
    [EntityIdentifier("CurrentShopperHasPurchaseItemCondition")]
    public class CurrentShopperHasPurchaseItemCondition : CartTargetItemId, ICustomerCondition
    {
       
        private readonly FindEntitiesInListCommand _getEntitFindEntitiesInListCommand;

        public CurrentShopperHasPurchaseItemCondition(FindEntitiesInListCommand findEntitiesInListCommand)
        {
            this._getEntitFindEntitiesInListCommand = findEntitiesInListCommand;
        }
        public IRuleValue<string> CurrentItemInCartId { get; set; }
        public bool Evaluate(IRuleExecutionContext context)
        {
            //itemId = ID to check in purchase order history
            //itemInCartItemId = Item in cart to check in order history to give discount
            //If customer has purchased itemId and has not purchased itemInCartItemId this rule returns true
            string itemId = this.TargetItemId.Yield(context);
            string itemInCartItemId = this.CurrentItemInCartId.Yield(context);
            bool valid = false;

            CommerceContext commerceContext = context.Fact<CommerceContext>((string)null);
                        
            if (commerceContext == null || !commerceContext.CurrentUserIsRegistered() || string.IsNullOrEmpty(itemId))
                return false;
            //UPDATED TO USE CURRENTSHOPPERID VS EMPTY CURRENTCUSTOMERID          
            string listName = string.Format(commerceContext.GetPolicy<KnownOrderListsPolicy>().CustomerOrders, (object)commerceContext.CurrentShopperId());
            CommerceList<Order> result = Task.Run<CommerceList<Order>>((Func<Task<CommerceList<Order>>>)(() => this._getEntitFindEntitiesInListCommand.Process<Order>(commerceContext, listName, 0, int.MaxValue))).Result;
            List<Order> source = result != null ? result.Items.ToList<Order>() : (List<Order>)null;
            if (source == null || !source.Any<Order>())
                return false;
            if (!this.MatchesAnItem(context))
            {
                if(
                source.Any<Order>(
                   (Func<Order, bool>)(
                   o => o.Lines.Any<CartLineComponent>(
                       (Func<CartLineComponent, bool>)(
                       l => l.ItemId.StartsWith(itemId, StringComparison.OrdinalIgnoreCase)
                       )))))
                {
                    valid = source.Where<Order>(
                   (Func<Order, bool>)(
                   o => o.Lines.Where<CartLineComponent>(
                       (Func<CartLineComponent, bool>)(
                       l => l.ItemId.Contains(itemInCartItemId)
                       )).FirstOrDefault() != null)).FirstOrDefault() == null;
                    return valid;
                }
            }
            if(source.Any<Order>(
                (Func<Order, bool>)(
                o => o.Lines.Any<CartLineComponent>(
                    (Func<CartLineComponent, bool>)(
                    l => l.ItemId.Equals(itemId)
                    )))))
            {
                valid = source.Where<Order>(
                   (Func<Order, bool>)(
                   o => o.Lines.Where<CartLineComponent>(
                       (Func<CartLineComponent, bool>)(
                       l => !l.ItemId.Equals(itemInCartItemId, StringComparison.OrdinalIgnoreCase)
                       )).FirstOrDefault() == null)).FirstOrDefault() == null;
                return valid;
            }
            return valid;
        }

    }
}
