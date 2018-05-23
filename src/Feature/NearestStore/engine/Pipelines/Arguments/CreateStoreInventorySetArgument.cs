using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments
{
    public class CreateStoreInventorySetArgument : PipelineArgument
    {
        public CreateStoreInventorySetArgument(string name, string displayName, string description)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.Description = description;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }


        // Store Details
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Abbreviation { get; set; }
        public string ZipCode { get; set; }
        public string CountryCode { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
    }
}
