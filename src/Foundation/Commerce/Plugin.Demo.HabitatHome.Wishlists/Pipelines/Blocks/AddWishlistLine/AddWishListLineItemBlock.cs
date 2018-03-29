using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using Plugin.Demo.HabitatHome.Wishlists.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Plugin.ManagedLists;
using System.Linq;
using Plugin.Demo.HabitatHome.Wishlists.Pipelines.Arguments;
using Sitecore.Commerce.Plugin.Carts;
using Plugin.Demo.HabitatHome.Wishlists.Components;

namespace Plugin.Demo.HabitatHome.Wishlists.Pipelines.Blocks.AddWishlistLine
{
    [PipelineDisplayName("Wishlists.AddWishListLineItemBlock")]
    public class AddWishListLineItemBlock : PipelineBlock<CartLineArgument, Cart, CommercePipelineExecutionContext>
    {
        public override async Task<Cart> Run(CartLineArgument arg, CommercePipelineExecutionContext context)
        {
            AddWishListLineItemBlock addCartLineBlock = this;
            
            Condition.Requires<CartLineArgument>(arg).IsNotNull<CartLineArgument>(string.Format("{0}: The argument cannot be null.", addCartLineBlock.Name));            
            Condition.Requires<Cart>(arg.Cart).IsNotNull<Cart>(string.Format("{0}: The cart cannot be null.", addCartLineBlock.Name));            
            Condition.Requires<CartLineComponent>(arg.Line).IsNotNull<CartLineComponent>(string.Format("{0}: The line to add cannot be null.", addCartLineBlock.Name));
            context.CommerceContext.AddObject((object)arg);
            Cart cart = arg.Cart;

            var cartComponent = cart.GetComponent<CartTypeComponent>();
            if (cartComponent == null)
            {
                var cartTypeComponent = new CartTypeComponent();
                cartTypeComponent.CartType = CartTypeEnum.Wishlist.ToString();
                cart.SetComponent(cartTypeComponent);
            }
            cartComponent.CartType = CartTypeEnum.Wishlist.ToString();


            LineQuantityPolicy lineQuantityPolicy = context.GetPolicy<LineQuantityPolicy>();
            Decimal quantity = arg.Line.Quantity;
            CartLineComponent existingLine = cart.Lines.FirstOrDefault<CartLineComponent>((Func<CartLineComponent, bool>)(l => l.ItemId.Equals(arg.Line.ItemId, StringComparison.Ordinal)));
            bool flag1 = !await lineQuantityPolicy.IsValid(quantity, context.CommerceContext);
            if (!flag1)
            {
                bool flag2 = existingLine != null;
                if (flag2)
                    flag2 = !await lineQuantityPolicy.IsValid(quantity + existingLine.Quantity, context.CommerceContext);
                flag1 = flag2;
            }
            if (flag1)
            {
                context.Abort("Invalid or missing value for property 'Quantity'.", (object)context);
                return cart;
            }
            if (!string.IsNullOrEmpty(arg.Line.ItemId))
            {
                if (arg.Line.ItemId.Split('|').Length >= 3)
                {
                    if (string.IsNullOrEmpty(arg.Line.Id))
                        arg.Line.Id = Guid.NewGuid().ToString("N");
                    List<CartLineComponent> list = cart.Lines.ToList<CartLineComponent>();
                    if (!context.CommerceContext.GetPolicy<RollupCartLinesPolicy>().Rollup)
                    {
                        list.Add(arg.Line);
                        context.CommerceContext.AddModel((Model)new LineAdded(arg.Line.Id, false));
                    }
                    else if (existingLine != null)
                    {
                        existingLine.Quantity += arg.Line.Quantity;
                        arg.Line.Id = existingLine.Id;
                        context.CommerceContext.AddModel((Model)new LineUpdated(arg.Line.Id));
                    }
                    else
                    {
                        list.Add(arg.Line);
                        context.CommerceContext.AddModel((Model)new LineAdded(arg.Line.Id, false));
                    }
                    cart.Lines = (IList<CartLineComponent>)list;
                    return cart;
                }
            }
            CommercePipelineExecutionContext executionContext = context;
            CommerceContext commerceContext = context.CommerceContext;
            string error = context.GetPolicy<KnownResultCodes>().Error;
            string commerceTermKey = "ItemIdIncorrectFormat";
            object[] args = new object[1]
            {
                (object) arg.Line.ItemId
            };
            string defaultMessage = string.Format("Expecting a CatalogId and a ProductId in the ItemId: {0}.", (object)arg.Line.ItemId);
            executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), (object)context);
            executionContext = (CommercePipelineExecutionContext)null;
            return cart;
        }

        public AddWishListLineItemBlock()
          : base((string)null)
        {
        }
    }
}
