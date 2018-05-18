using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines
{
    public class AddWishListLineItemPipeline : CommercePipeline<CartLineArgument, Cart>, IAddWishListLineItemPipeline, IPipeline<CartLineArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock<CartLineArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public AddWishListLineItemPipeline(IPipelineConfiguration<IAddWishListLineItemPipeline> configuration, ILoggerFactory loggerFactory)
      : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}
