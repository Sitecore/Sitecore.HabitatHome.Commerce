// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpgradetConstants.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    /// <summary>
    /// The Upgradet constants.
    /// </summary>
    public static class UpgradeConstants
    {
        /// <summary>
        /// The name of the Upgradet pipelines.
        /// </summary>
        public static class Pipelines
        {
            /// <summary>
            /// The migrate environment
            /// </summary>
            public const string MigrateEnvironment = "Upgrade.pipelines.migrateenvironment";

            /// <summary>
            /// The migrate environment metadata
            /// </summary>
            public const string MigrateEnvironmentMetadata = "Upgrade.pipelines.migrateenvironmentmetadata";

            /// <summary>
            /// The migrate entity metadata
            /// </summary>
            public const string MigrateEntityMetadata = "Upgrade.pipelines.migrateentitymetadata";

            /// <summary>
            /// The migrate list
            /// </summary>
            public const string MigrateList = "Upgrade.pipelines.migratelist";

            /// <summary>
            /// The name of the Upgradet pipeline blocks.
            /// </summary>
            public static class Blocks
            {
                /// <summary>
                /// The configure Ops service API block name.
                /// </summary>
                public const string ConfigureOpsServiceApiBlock = "Upgrade.ConfigureOpsServiceApi";

                /// <summary>
                /// The get source environment block
                /// </summary>
                public const string GetSourceEnvironmentBlock = "Upgrade.GetSourceEnvironment";

                /// <summary>
                /// The inject environment policies block
                /// </summary>
                public const string InjectEnvironmentPoliciesBlock = "Upgrade.InjectEnvironmentPolicies";

                /// <summary>
                /// The migrate lists block
                /// </summary>
                public const string MigrateListsBlock = "Upgrade.MigrateLists";

                /// <summary>
                /// The migrate list block
                /// </summary>
                public const string MigrateListBlock = "Upgrade.MigrateList";

                /// <summary>
                /// The get source entity block
                /// </summary>
                public const string GetSourceEntityBlock = "Upgrade.GetSourceEntity";

                /// <summary>
                /// The get target entity block
                /// </summary>
                public const string GetTargetEntityBlock = "Upgrade.GetTargetEntityBlock";

                /// <summary>
                /// The migrate order entity block
                /// </summary>
                public const string MigrateOrderEntityBlock = "Upgrade.MigrateOrderEntity";

                /// <summary>
                /// The migrate entity index block
                /// </summary>
                public const string MigrateEntityIndexBlock = "Upgrade.MigrateEntityIndex";

                /// <summary>
                /// The migrate gift card block
                /// </summary>
                public const string MigrateGiftCardBlock = "Upgrade.MigrateGiftCard";

                /// <summary>
                /// The migrate journal entry block
                /// </summary>
                public const string MigrateJournalEntryBlock = "Upgrade.MigrateJournalEntry";

                /// <summary>
                /// The migrate sellable item block
                /// </summary>
                public const string MigrateSellableItemBlock = "Upgrade.MigrateSellableItem";

                /// <summary>
                /// The patch environment json block
                /// </summary>
                public const string PatchEnvironmentJsonBlock = "Upgrade.PatchEnvironmentJson";

                /// <summary>
                /// The set entity list memberships block
                /// </summary>
                public const string SetEntityListMembershipsBlock = "Upgrade.SetEntityListMemberships";

                /// <summary>
                /// The persist migrated entity block
                /// </summary>
                public const string PersistMigratedEntityBlock = "Upgrade.PersistMigratedEntity";

                /// <summary>
                /// The finalize environment migration block
                /// </summary>
                public const string FinalizeEnvironmentMigrationBlock = "Upgrade.FinalizeEnvironmentMigration";
            }
        }
    }
}
