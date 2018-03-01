// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilesCsCachePolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using Sitecore.Commerce.Core;

    /// <summary>
    /// Defines the cs profiles cache policy
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.CachePolicy" />
    public class ProfilesCsCachePolicy : CachePolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilesCsCachePolicy"/> class.
        /// </summary>
        public ProfilesCsCachePolicy()
        {
            this.CacheName = "Profiles";
        }
    }
}
