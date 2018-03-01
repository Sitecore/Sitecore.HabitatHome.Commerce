// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using System.Collections.Generic;

    /// <summary>
    /// SQL policy for the upgrade migration
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Policy" />
    public class MigrationPolicy : Policy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationPolicy"/> class.
        /// </summary>
        public MigrationPolicy()
        {
            this.DefaultEnvironmentName = "AdventureWorksAuthoring";
            this.SqlPolicySetName = "SqlPolicySet";
            this.ReviewOnly = false;

            this.ListsToMigrate = new Dictionary<string, int?>()
            {
                { "anonymousorders", 0},
                { "authenticatedorders", 0},
                { "completedorders", 0},
                { "coupons", 0},
                { "entitlements", 0},
                { "giftcards", 0},
                { "journalentries", 0},
                { "managedlistss", 0},
                { "onholdorders", 0},
                { "pricebooks", 0},
                { "pricecards", 0},
                { "privatecoupongroups", 0},
                { "promotion", 0},
                { "promotionbooks", 0},
                { "promotions", 0},
                { "releasedorders", 0},
                { "salesactivities", 0},
                { "sellableitems", 0},
                { "settledsalesactivities", 0}
            };
        }

        /// <summary>
        /// Gets or sets the name of the OOB new engine environment.
        /// </summary>
        /// <value>
        /// The name of the OOB new engine environment.
        /// </value>
        public string DefaultEnvironmentName { get; set; }

        /// <summary>
        /// Gets or sets the name of the SQL policy set.
        /// </summary>
        /// <value>
        /// The name of the SQL policy set.
        /// </value>
        public string SqlPolicySetName { get; set; }

        /// <summary>
        /// Gets or sets the lists to migrate.
        /// </summary>
        /// <value>
        /// The lists to migrate.
        /// </value>
        public Dictionary<string, int?> ListsToMigrate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [review only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [review only]; otherwise, <c>false</c>.
        /// </value>
        public bool ReviewOnly { get; set; }
    }
}
