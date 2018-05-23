using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments
{
    public class GetNearestStoreDetailsByLocationArgument : PipelineArgument
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
            
    }
}
