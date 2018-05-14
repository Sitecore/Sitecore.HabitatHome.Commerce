using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Pricing;

namespace Plugin.Demo.HabitatHome.Bundles.Pipelines.Blocks
{
    [PipelineDisplayName("Carts.AddCartLineBundlesBlock")]
    class AddToCartBundlesBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;


        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires<Cart>(arg).IsNotNull<Cart>(string.Format("{0}: the cart can not be null.", (object)this.Name));
            if (arg.HasComponent<TemporaryCartComponent>())
                return arg;

            AddToCartBundlesBlock addBundlesBlock = this;
            context.CommerceContext.AddObject((object)arg);
            Cart cart = arg;

            FindEntityArgument getSavedCartArg = new FindEntityArgument(typeof(Cart), arg.Id, false);
            Cart savedCart = await this._findEntityPipeline.Run(getSavedCartArg, (CommercePipelineExecutionContext)context).ConfigureAwait(false) as Cart;

            CartLineComponent existingLine;

            IList<CartLineComponent> savedCartLines = new List<CartLineComponent>();
            IList<CartLineComponent> currentCartLines = new List<CartLineComponent>();

            if (savedCart == null)
            {
                existingLine = arg.Lines.FirstOrDefault<CartLineComponent>();
            }
            else
            {
                savedCartLines = savedCart.Lines;
                currentCartLines = cart.Lines;
                var addedCartLine = cart.Lines.Where(l => savedCartLines.Where(sc => sc.Id == l.Id).FirstOrDefault() == null).FirstOrDefault();
                existingLine = addedCartLine;

            }
            if (existingLine != null)
            {
                FindEntityArgument getProductArg = new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-" + (existingLine.ItemId.Split('|').Count() > 1 ? existingLine.ItemId.Split('|')[1] : existingLine.ItemId), false);
                SellableItem carLineProduct = await this._findEntityPipeline.Run(getProductArg, (CommercePipelineExecutionContext)context).ConfigureAwait(false) as SellableItem;

                bool hasTag = carLineProduct.Tags.Any<Tag>((Func<Tag, bool>)(t => t.Name.Equals("bundle", StringComparison.OrdinalIgnoreCase)));

                if (hasTag)
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
                        if (savedCartLines.Any(l => l.ItemId.Contains(relProd.FriendlyId)) || currentCartLines.Any(l => l.ItemId.Contains(relProd.FriendlyId)))
                        {
                            FindEntityArgument getRelatedProductArg = new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-" + relProd.FriendlyId, false);
                            SellableItem relatedProduct = await this._findEntityPipeline.Run(getRelatedProductArg, (CommercePipelineExecutionContext)context).ConfigureAwait(false) as SellableItem;

                            string listPrice = String.Empty;
                            if (relatedProduct.HasPolicy<ListPricingPolicy>())
                            {
                                ListPricingPolicy policy = relatedProduct.GetPolicy<ListPricingPolicy>();
                                listPrice = policy.Prices.FirstOrDefault().Amount.ToString();
                            }                                
                            existingLine.Comments += relProd.FriendlyId + ',' + relProd.DisplayName + ',' + listPrice + '|';  
                        }
                    }
                }
            }
            return cart;
        }

        public AddToCartBundlesBlock(IFindEntityPipeline findEntityPipeline, IFindEntitiesInListPipeline findEntitiesInListPipeline) : base((string)null)
        {
            this._findEntityPipeline = findEntityPipeline;
            _findEntitiesInListPipeline = findEntitiesInListPipeline;
        }
    }
}
