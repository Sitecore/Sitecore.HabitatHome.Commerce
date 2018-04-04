using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.Plugin.Carts;

namespace Plugin.Demo.HabitatHome.Wishlists.Pipelines
{  

    [PipelineDisplayName("pipelines:addcartline")]
    public interface IAddWishListLineItemPipeline : IPipeline<CartLineArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock<CartLineArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }
}
