namespace Plugin.Demo.HabitatHome
{
    /// <summary>
    /// The Habitat constants.
    /// </summary>
    public static class HabitatHomeConstants
    {
        /// <summary>
        /// The name of the Habitat pipelines.
        /// </summary>
        public static class Pipelines
        {
            /// <summary>
            /// The name of the Habitat pipeline blocks.
            /// </summary>
            public static class Blocks
            {
                /// <summary>
                /// The initialize catalog block name.
                /// </summary>
                public const string InitializeCatalogBlock = "HabitatHome.block.InitializeCatalog";

                /// <summary>
                /// The registered plugin block name.
                /// </summary>
                public const string RegisteredPluginBlock = "HabitatHome.block.RegisteredPlugin";

                /// <summary>
                /// The initialize sellable items block name.
                /// </summary>
                public const string InitializeSellableItemsBlock = "HabitatHome.block.InitializeEnvironmentSellableItems";

                public const string InitializePricingBlock = "HabitatHome.block.InitializeEnvironmentPricing";

                public const string InitializePromotionsBlock = "HabitatHome.block.InitializeEnvironmentPromotions";
                                                              
            }
        }
    }
}
