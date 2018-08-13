using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Entities;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines
{            
    public class GetWishlistPipeline : CommercePipeline<GetWishListArgument, WishList>, IGetWishlistPipeline
    {
        public GetWishlistPipeline(IPipelineConfiguration<IGetWishlistPipeline> configuration, ILoggerFactory loggerFactory)
          : base(configuration, loggerFactory)
        {
        }
    }
}
