using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Plugin.Demo.HabitatHome.Wishlists.Pipelines.Arguments;
using Plugin.Demo.HabitatHome.Wishlists.Entities;

namespace Plugin.Demo.HabitatHome.Wishlists.Pipelines
{
    [PipelineDisplayName("pipelines:getWishlist")]
    public interface IGetWishlistPipeline : IPipeline<GetWishListArgument, WishList, CommercePipelineExecutionContext>, IPipelineBlock<GetWishListArgument, WishList, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }   
}
