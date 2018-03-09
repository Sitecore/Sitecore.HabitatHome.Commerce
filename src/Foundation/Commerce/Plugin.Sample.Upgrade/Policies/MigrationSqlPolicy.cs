// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationSqlPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.SQL;
   
    /// <summary>
    /// SQL policy for the upgrade migration
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Policy" />
    public class MigrationSqlPolicy : Policy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationSqlPolicy"/> class.
        /// </summary>
        public MigrationSqlPolicy()
        {
            this.ArtifactStoreId = "6BE385F1-93DC-4299-9DD4-934F6BA42EAA";
            this.SourceStoreSqlPolicy = new EntityStoreSqlPolicy {
                Server = ".",
                Database = "SitecoreCommerce_Global", 
                TrustedConnection = true
            };
        }

        /// <summary>
        /// Gets or sets the global artifact store identifier.
        /// </summary>
        /// <value>
        /// The artifact store identifier.
        /// </value>
        public string ArtifactStoreId { get; set; }

        /// <summary>
        /// Gets or sets the source store SQL policy.
        /// </summary>
        /// <value>
        /// The source store SQL policy.
        /// </value>
        public EntityStoreSqlPolicy SourceStoreSqlPolicy { get; set; }
    }
}
