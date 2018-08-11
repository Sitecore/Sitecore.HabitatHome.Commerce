using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines
{                                          
    public class RemoveWishListLinePipeline : CommercePipeline<CartLineArgument, Cart>, IRemoveWishListLinePipeline
    {
        public RemoveWishListLinePipeline(IPipelineConfiguration<IRemoveWishListLinePipeline> configuration, ILoggerFactory loggerFactory)
          : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}
