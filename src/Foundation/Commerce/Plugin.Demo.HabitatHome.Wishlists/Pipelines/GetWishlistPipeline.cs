using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Plugin.Demo.HabitatHome.Wishlists.Pipelines.Arguments;
using Plugin.Demo.HabitatHome.Wishlists.Entities;

namespace Plugin.Demo.HabitatHome.Wishlists.Pipelines
{
   
    public class GetWishlistPipeline : CommercePipeline<GetWishListArgument, WishList>, IGetWishlistPipeline, IPipeline<GetWishListArgument, WishList, CommercePipelineExecutionContext>, IPipelineBlock<GetWishListArgument, WishList, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public GetWishlistPipeline(IPipelineConfiguration<IGetWishlistPipeline> configuration, ILoggerFactory loggerFactory)
          : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}
