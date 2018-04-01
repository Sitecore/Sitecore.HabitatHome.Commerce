using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Arguments
{
    public class SellableItemInventorySetsArgument : PipelineArgument
    {
        public string SellableItemId { get; set; }

        public string VariationId { get; set; }

        public List<string> InventorySetIds { get; set; }
       
    }
}
