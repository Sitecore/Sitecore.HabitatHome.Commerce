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
            Condition.Requires<CartLineArgument>(arg).IsNotNull<CartLineArgument>("The argument can not be null");
            Condition.Requires<Cart>(arg.Cart).IsNotNull<Cart>("The cart can not be null");
            Condition.Requires<CartLineComponent>(arg.Line).IsNotNull<CartLineComponent>("The lines can not be null");
            Cart cart = arg.Cart;
            List<CartLineComponent> lines = cart.Lines.ToList<CartLineComponent>();
            CartLineComponent existingLine = lines.FirstOrDefault<CartLineComponent>((Func<CartLineComponent, bool>)(l => l.Id.Equals(arg.Line.Id, StringComparison.OrdinalIgnoreCase)));
            if (existingLine != null)
            {
                string str = await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Information, (string)null, (object[])null, string.Format("Removed Line '{0}' from Cart '{1}'.", (object)existingLine.Id, (object)cart.Id));
                lines.Remove(existingLine);
            }
            cart.Lines = (IList<CartLineComponent>)lines;
            return cart;
        }

        public RemoveWishlistLineBlock()
          : base((string)null)
        {
        }
    }
}
