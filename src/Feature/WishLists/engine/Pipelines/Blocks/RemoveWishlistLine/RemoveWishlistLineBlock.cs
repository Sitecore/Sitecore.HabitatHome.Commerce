using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines.Blocks.RemoveWishlistLine
{
    [PipelineDisplayName("Wishlists.AddWishListLineItemBlock")]
    public class RemoveWishlistLineBlock : PipelineBlock<CartLineArgument, Cart, CommercePipelineExecutionContext>
    {                      
        public override async Task<Cart> Run(CartLineArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull("The argument can not be null");
            Condition.Requires(arg.Cart).IsNotNull("The cart can not be null");
            Condition.Requires(arg.Line).IsNotNull("The lines can not be null");

            Cart cart = arg.Cart;
            List<CartLineComponent> lines = cart.Lines.ToList();
            CartLineComponent existingLine = lines.FirstOrDefault(l => l.Id.Equals(arg.Line.Id, StringComparison.OrdinalIgnoreCase));

            if (existingLine != null)
            {
                string str = await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Information, null, null, $"Removed Line '{existingLine.Id}' from Wishlist '{cart.Id}'.");
                lines.Remove(existingLine);
            }

            cart.Lines = lines;
            return cart;
        }
    }
}
