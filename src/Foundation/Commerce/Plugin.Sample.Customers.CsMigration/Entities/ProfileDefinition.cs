// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileDefinition.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   A ProfileDefinition Commerce entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using Sitecore.Commerce.Core;
    using System.Collections.Generic;    

    /// <summary>
    /// A ProfileDefinition Commerce entity
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.CommerceEntity" />
    public class ProfileDefinition : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileDefinition"/> class.
        /// </summary>
        public ProfileDefinition()
        {
            this.Properties = new List<ProfileProperty>();
        }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>       
        public IList<ProfileProperty> Properties { get; set; }
    }
}
