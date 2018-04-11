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
using Sitecore.Commerce.Plugin.Carts;

namespace Plugin.Demo.HabitatHome.Wishlists.Pipelines
{
    public class AddWishListLineItemPipeline : CommercePipeline<CartLineArgument, Cart>, IAddWishListLineItemPipeline, IPipeline<CartLineArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock<CartLineArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public AddWishListLineItemPipeline(IPipelineConfiguration<IAddWishListLineItemPipeline> configuration, ILoggerFactory loggerFactory)
      : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}
