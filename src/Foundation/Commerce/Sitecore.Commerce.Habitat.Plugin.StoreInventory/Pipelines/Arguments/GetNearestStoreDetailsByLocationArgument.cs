using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Arguments
{
    public class GetNearestStoreDetailsByLocationArgument : PipelineArgument
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
            
    }
}
