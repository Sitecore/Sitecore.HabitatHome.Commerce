﻿using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines
{
    public class AssociateStoreInventoryToSellableItem :   CommercePipeline<SellableItemInventorySetsArgument, bool>, IAssociateStoreInventoryToSellableItem, IPipeline<SellableItemInventorySetsArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock<SellableItemInventorySetsArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public AssociateStoreInventoryToSellableItem(IPipelineConfiguration<IAssociateStoreInventoryToSellableItem> configuration, ILoggerFactory loggerFactory) : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}
