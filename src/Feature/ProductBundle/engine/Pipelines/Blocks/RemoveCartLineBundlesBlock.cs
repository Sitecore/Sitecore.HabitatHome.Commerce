using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.ProductBundle.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Carts.RemoveCartLineBundlesBlock")]
    class RemoveCartLineBundlesBlock : PipelineBlock<CartLineArgument, CartLineArgument, CommercePipelineExecutionContext>
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;


        public override async Task<CartLineArgument> Run(CartLineArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires<CartLineArgument>(arg).IsNotNull<CartLineArgument>("The argument can not be null");
            Condition.Requires<Cart>(arg.Cart).IsNotNull<Cart>("The cart can not be null");
            Condition.Requires<CartLineComponent>(arg.Line).IsNotNull<CartLineComponent>("The lines can not be null");
            Cart cart = arg.Cart;
          
            List<CartLineComponent> lines = cart.Lines.ToList<CartLineComponent>();
            CartLineComponent existingLine = lines.FirstOrDefault<CartLineComponent>((Func<CartLineComponent, bool>)(l => l.Id.Equals(arg.Line.Id, StringComparison.OrdinalIgnoreCase)));
            if (existingLine != null)
            {
                FindEntityArgument getProductArg = new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-" + (existingLine.ItemId.Split('|').Count() > 1 ? existingLine.ItemId.Split('|')[1] : existingLine.ItemId), false);
                SellableItem carLineProduct = await this._findEntityPipeline.Run(getProductArg, (CommercePipelineExecutionContext)context).ConfigureAwait(false) as SellableItem;

                bool hasTag = carLineProduct.Tags.Any<Tag>((Func<Tag, bool>)(t => t.Name.Equals("bundle", StringComparison.OrdinalIgnoreCase)));
                if(hasTag)
                {
                    string listId = String.Format("relatedproduct-{0}", existingLine.ItemId.Split('|').Count() > 1 ? existingLine.ItemId.Split('|')[1] : existingLine.ItemId);
                    var relatedProducts = await _findEntitiesInListPipeline.Run(
                        new FindEntitiesInListArgument(typeof(CommerceEntity), listId, 0, 10)
                        {
                            LoadEntities = true
                        },
                        context);
                    foreach (var relProd in relatedProducts.List.Items)
                    {
                        if (cart.Lines.Any(l => l.ItemId.Contains(relProd.FriendlyId)))
                        {
                            var relatedProductCartLine = cart.Lines.FirstOrDefault<CartLineComponent>(l => l.ItemId.Contains(relProd.FriendlyId));
                            if (relatedProductCartLine != null)
                                cart.Lines.Remove(relatedProductCartLine);
                        }
                    }
                }
            }
            return arg;
        }

        public RemoveCartLineBundlesBlock(IFindEntityPipeline findEntityPipeline, IFindEntitiesInListPipeline findEntitiesInListPipeline) : base((string)null)
        {
            this._findEntityPipeline = findEntityPipeline;
            _findEntitiesInListPipeline = findEntitiesInListPipeline;
        }
    }
}
