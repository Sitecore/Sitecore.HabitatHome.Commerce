﻿using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines
{
    

    public class RemoveWishListLinePipeline : CommercePipeline<CartLineArgument, Cart>, IRemoveWishListLinePipeline, IPipeline<CartLineArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock<CartLineArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public RemoveWishListLinePipeline(IPipelineConfiguration<IRemoveWishListLinePipeline> configuration, ILoggerFactory loggerFactory)
          : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}
