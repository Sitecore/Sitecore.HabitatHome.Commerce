using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Entities;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines
{
    [PipelineDisplayName("pipelines:getWishlist")]
    public interface IGetWishlistPipeline : IPipeline<GetWishListArgument, WishList, CommercePipelineExecutionContext>, IPipelineBlock<GetWishListArgument, WishList, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }   
}
