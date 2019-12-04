using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.Catalog.Engine.Pipelines.Blocks
{
    [PipelineDisplayName(HabitatHomeConstants.Pipelines.Blocks.PopulateLineItemProductBlock)]
    public class PopulateLineItemProductBlock : PipelineBlock<CartLineComponent, CartLineComponent, CommercePipelineExecutionContext>
    {
        public override async Task<CartLineComponent> Run(CartLineComponent arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The line can not be null");

            var productArgument = ProductArgument.FromItemId(arg.ItemId);

            SellableItem sellableItem = null;
            if (productArgument.IsValid())
            {
                sellableItem =
                    context.CommerceContext.GetEntity<SellableItem>(s =>
                        s.ProductId.Equals(productArgument.ProductId, StringComparison.OrdinalIgnoreCase));
            }

            if (sellableItem == null)
                return null;

            var productComponent = arg.GetComponent<CartProductComponent>();
            productComponent.ItemType = sellableItem.TypeOfGood;

            return arg;
        }
    }
}
