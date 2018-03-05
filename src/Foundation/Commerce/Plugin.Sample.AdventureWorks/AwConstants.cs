// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwConstants.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.AdventureWorks
{
    /// <summary>
    /// The adventure works  constants.
    /// </summary>
    public static class AwConstants
    {
        /// <summary>
        /// The names of the adventure works  pipelines.
        /// </summary>
        public static class Pipelines
        {
            /// <summary>
            /// The names of the adventure works  pipelines blocks.
            /// </summary>
            public static class Blocks
            {
                /// <summary>
                /// The bootstrap aw  minions block name.
                /// </summary>
                public const string BootstrapAwMinionsBlock = "AdventureWorks:block:bootstrapawminions";

                /// <summary>
                /// The initialize catalog block name.
                /// </summary>
                public const string InitializeCatalogBlock = "AdventureWorks.block.InitializeCatalog";

                /// <summary>
                /// The InitializeEnvironmentGiftCardsBlock name.
                /// </summary>
                public const string InitializeEnvironmentGiftCardsBlock = "AdventureWorks.InitializeEnvironmentGiftCardsBlock";

                /// <summary>
                /// The initialize sellable items block name.
                /// </summary>
                public const string InitializeSellableItemsBlock = "AdventureWorks.block.InitializeEnvironmentSellableItems";

                /// <summary>
                /// The bootstrap aw  pricing block name.
                /// </summary>
                public const string InitializeEnvironmentPricingBlock = "AdventureWorks.InitializeEnvironmentPricingBlock";
                
                /// <summary>
                /// The bootstrap aw  promotions block name.
                /// </summary>
                public const string InitializeEnvironmentPromotionsBlock = "AdventureWorks.InitializeEnvironmentPromotionsBlock";

                /// <summary>
                /// The bootstrap aw  shops block name.
                /// </summary>
                public const string InitializeEnvironmentShopsBlock = "AdventureWorks.InitializeEnvironmentShopsBlock";

                /// <summary>
                /// The bootstrap aw  regions block name.
                /// </summary>
                public const string InitializeEnvironmentRegionsBlock = "AdventureWorks.InitializeEnvironmentRegionsBlock";

                /// <summary>
                /// The bootstrap adventure works  block name.
                /// </summary>
                public const string BootstrapAdventureWorksBlock = "AdventureWorks:block:bootstrapaw";

                /// <summary>
                /// The bootstrap aw  policy sets block name.
                /// </summary>
                public const string InitializeEnvironmentPolicySetsBlock = "AdventureWorks.InitializeEnvironmentPolicySetsBlock";

                /// <summary>
                /// The registered plugin block name.
                /// </summary>
                public const string RegisteredPluginBlock = "AdventureWorks:block:registeredplugin";
                
                /// <summary>
                /// The Bootstrap ManagedLists Block Name.
                /// </summary>
                public const string BootstrapManagedListsBlock = "Orders.block.BootstrapManagedListsBlock";
            }
        }
    }
}
