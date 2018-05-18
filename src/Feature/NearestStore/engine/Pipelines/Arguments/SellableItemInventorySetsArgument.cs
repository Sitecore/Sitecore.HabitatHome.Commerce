using System.Collections.Generic;
using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments
{
    public class SellableItemInventorySetsArgument : PipelineArgument
    {
        public string SellableItemId { get; set; }

        public string VariationId { get; set; }

        public List<string> InventorySetIds { get; set; }
       
    }
}
