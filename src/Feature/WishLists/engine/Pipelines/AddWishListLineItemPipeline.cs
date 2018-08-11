using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines
{
    public class AddWishListLineItemPipeline : CommercePipeline<CartLineArgument, Cart>, IAddWishListLineItemPipeline
    {
        public AddWishListLineItemPipeline(IPipelineConfiguration<IAddWishListLineItemPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}
