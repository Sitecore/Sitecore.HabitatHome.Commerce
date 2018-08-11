using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Components;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Entities;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines.Blocks.AddWishlistLine
{
    [PipelineDisplayName("Wishlists.AddWishListLineItemBlock")]
    public class AddWishListLineBlock : PipelineBlock<CartLineArgument, Cart, CommercePipelineExecutionContext>
    {
        public AddWishListLineBlock()
        {
        }

        public override async Task<Cart> Run(CartLineArgument arg, CommercePipelineExecutionContext context)
        {
            AddWishListLineBlock addCartLineBlock = this;
            
            Condition.Requires<CartLineArgument>(arg).IsNotNull<CartLineArgument>($"{addCartLineBlock.Name}: The argument cannot be null.");            
            Condition.Requires<Cart>(arg.Cart).IsNotNull<Cart>($"{addCartLineBlock.Name}: The cart cannot be null.");            
            Condition.Requires<CartLineComponent>(arg.Line).IsNotNull<CartLineComponent>($"{addCartLineBlock.Name}: The line to add cannot be null.");
            context.CommerceContext.AddObject(arg);
            Cart cart = arg.Cart;

            var cartComponent = cart.GetComponent<CartTypeComponent>();
            if (cartComponent == null)
            {
                cartComponent = new CartTypeComponent();
                cartComponent.CartType = CartTypeEnum.Wishlist.ToString();
                cart.SetComponent(cartComponent);
            }

            cartComponent.CartType = CartTypeEnum.Wishlist.ToString();    

            LineQuantityPolicy lineQuantityPolicy = context.GetPolicy<LineQuantityPolicy>();
            Decimal quantity = arg.Line.Quantity;
            CartLineComponent existingLine = cart.Lines.FirstOrDefault(l => l.ItemId.Equals(arg.Line.ItemId, StringComparison.Ordinal));

            bool isValidQuantity = await lineQuantityPolicy.IsValid(quantity, context.CommerceContext);
            if (isValidQuantity)
            {
                if (existingLine != null)
                {
                    isValidQuantity = await lineQuantityPolicy.IsValid(quantity + existingLine.Quantity, context.CommerceContext);
                }                           
            }       

            if (!isValidQuantity)
            {
                context.Abort("Invalid or missing value for property 'Quantity'.", context);
                return cart;
            }

            if (!string.IsNullOrEmpty(arg.Line.ItemId))
            {
                if (arg.Line.ItemId.Split('|').Length >= 3)
                {
                    if (string.IsNullOrEmpty(arg.Line.Id))
                    {
                        arg.Line.Id = Guid.NewGuid().ToString("N");
                    }

                    List<CartLineComponent> list = cart.Lines.ToList();

                    if (!context.CommerceContext.GetPolicy<RollupCartLinesPolicy>().Rollup)
                    {
                        list.Add(arg.Line);
                        context.CommerceContext.AddModel(new LineAdded(arg.Line.Id));
                    }
                    else if (existingLine != null)
                    {
                        existingLine.Quantity += arg.Line.Quantity;
                        arg.Line.Id = existingLine.Id;
                        context.CommerceContext.AddModel(new LineUpdated(arg.Line.Id));
                    }
                    else
                    {
                        list.Add(arg.Line);
                        context.CommerceContext.AddModel(new LineAdded(arg.Line.Id));
                    }

                    cart.Lines = list;
                    return cart;
                }
            }

            CommercePipelineExecutionContext executionContext = context;
            CommerceContext commerceContext = context.CommerceContext;
            string error = context.GetPolicy<KnownResultCodes>().Error;
            string commerceTermKey = "ItemIdIncorrectFormat";
            object[] args = new object[1]
            {
                arg.Line.ItemId
            };
            string defaultMessage = $"Expecting a CatalogId and a ProductId in the ItemId: {arg.Line.ItemId}.";
            executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), context);
            executionContext = (CommercePipelineExecutionContext)null;
            return cart;
        }
    }
}
