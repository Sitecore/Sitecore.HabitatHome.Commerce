using System;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.ProductBundle.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Carts.HideCartLineBundlesBlock")]
    class HideCartLineBundlesBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;


        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires<Cart>(arg).IsNotNull<Cart>(string.Format("{0}: the cart can not be null.", (object)this.Name));
            if (arg.HasComponent<TemporaryCartComponent>())
                return arg;
            context.CommerceContext.AddObject((object)arg);
            Cart cart = arg;

            foreach (var cl in cart.Lines)
            {
                FindEntityArgument getProductArg = new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-" + (cl.ItemId.Split('|').Count() > 1 ? cl.ItemId.Split('|')[1] : cl.ItemId), false);
                SellableItem carLineProduct = await this._findEntityPipeline.Run(getProductArg, (CommercePipelineExecutionContext)context).ConfigureAwait(false) as SellableItem;

                bool hasTag = carLineProduct.Tags.Any<Tag>((Func<Tag, bool>)(t => t.Name.Equals("bundle", StringComparison.OrdinalIgnoreCase)));
                if (hasTag)
                {
                    string listId = String.Format("relatedproduct-{0}", cl.ItemId.Split('|').Count() > 1 ? cl.ItemId.Split('|')[1] : cl.ItemId);
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
            return cart;
        }

        public HideCartLineBundlesBlock(IFindEntityPipeline findEntityPipeline, IFindEntitiesInListPipeline findEntitiesInListPipeline) : base((string)null)
        {
            this._findEntityPipeline = findEntityPipeline;
            _findEntitiesInListPipeline = findEntitiesInListPipeline;
        }
    }
}
