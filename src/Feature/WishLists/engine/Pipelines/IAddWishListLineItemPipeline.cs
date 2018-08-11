using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines
{            
    [PipelineDisplayName("pipelines:addcartline")]
    public interface IAddWishListLineItemPipeline : IPipeline<CartLineArgument, Cart, CommercePipelineExecutionContext>
    {
    }
}
