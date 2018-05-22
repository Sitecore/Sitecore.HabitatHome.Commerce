using System;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.ProductKit.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Carts.AddCartLineKitsBlock")]
    public class AddToCartKitsBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;
     

        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires<Cart>(arg).IsNotNull<Cart>(string.Format("{0}: the cart can not be null.", (object)this.Name));
            if (arg.HasComponent<TemporaryCartComponent>())
                return arg;

            AddToCartKitsBlock addKitsBlock = this;
            context.CommerceContext.AddObject((object)arg);
            Cart cart = arg;               

            FindEntityArgument getSavedCartArg = new FindEntityArgument(typeof(Cart), arg.Id, false);
            Cart savedCart = await this._findEntityPipeline.Run(getSavedCartArg, (CommercePipelineExecutionContext)context).ConfigureAwait(false) as Cart;

            CartLineComponent existingLine;

            if (savedCart == null)
            {
                existingLine = arg.Lines.FirstOrDefault<CartLineComponent>();
            }
            else
            {
                var savedCartLines = savedCart.Lines;
                var currentCartLines = cart.Lines;
                var addedCartLine = cart.Lines.Where(l => savedCartLines.Where(sc => sc.Id == l.Id).FirstOrDefault() == null).FirstOrDefault();
                existingLine = addedCartLine;

            }
            if (existingLine != null)
            {
               
                //var cartLineProductComponent = existingLine.GetComponent<CartProductComponent>();
                //bool hasTag = cartLineProductComponent.Tags.Any<Tag>((Func<Tag, bool>)(t => t.Name.Equals("kit", StringComparison.OrdinalIgnoreCase)));

                FindEntityArgument getProductArg = new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-" + (existingLine.ItemId.Split('|').Count() > 1 ? existingLine.ItemId.Split('|')[1] : existingLine.ItemId), false);
                SellableItem carLineProduct = await this._findEntityPipeline.Run(getProductArg, (CommercePipelineExecutionContext)context).ConfigureAwait(false) as SellableItem;

                bool hasTag = carLineProduct.Tags.Any<Tag>((Func<Tag, bool>)(t => t.Name.Equals("kit", StringComparison.OrdinalIgnoreCase)));

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
                        existingLine.Comments += relProd.Id + ',' + relProd.DisplayName + '|';
                    }
                }                             
            }

            return cart;
        }

        public AddToCartKitsBlock(IFindEntityPipeline findEntityPipeline, IFindEntitiesInListPipeline findEntitiesInListPipeline) : base((string)null)
        {
            this._findEntityPipeline = findEntityPipeline;
            _findEntitiesInListPipeline = findEntitiesInListPipeline;
        }
    }
}
