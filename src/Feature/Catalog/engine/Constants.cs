namespace Sitecore.HabitatHome.Feature.Catalog.Engine
{
    public static class HabitatHomeConstants
    {
        public static class Pipelines
        {
            public static class Blocks
            {
                public const string InitializeCatalogBlock = "Habitat.block.InitializeCatalog";
                public const string RegisteredPluginBlock = "Habitat.block.RegisteredPlugin";
                public const string InitializeSellableItemsBlock = "Habitat.block.InitializeEnvironmentSellableItems";
                public const string InitializePricingBlock = "Habitat.block.InitializeEnvironmentPricing";
                public const string InitializePromotionsBlock = "Habitat.block.InitializeEnvironmentPromotions";
                public const string InitializeBundlesBlock = "Habitat.block.InitializeBundles";
                public const string PopulateLineItemProductBlock = "Habitat.block.PopulateLineItemProductBlock";
            }
        }
    }
}
