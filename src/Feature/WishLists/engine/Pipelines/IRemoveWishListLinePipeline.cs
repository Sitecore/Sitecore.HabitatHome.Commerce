using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines
{                                
    [PipelineDisplayName("pipelines:removewishlistline")]
    public interface IRemoveWishListLinePipeline : IPipeline<CartLineArgument, Cart, CommercePipelineExecutionContext>
    {
    }
}
