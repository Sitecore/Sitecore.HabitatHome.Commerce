// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleEntity.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Demo.HabitatHome.Wishlists
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.OData.Builder;
    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// SampleEntity model.
    /// </summary>
    public class SampleEntity : CommerceEntity
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Demo.HabitatHome.Wishlists.SampleEntity" /> class.
        /// </summary>
        public SampleEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Demo.HabitatHome.Wishlists.SampleEntity" /> class. 
        /// Public Constructor
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public SampleEntity(string id) : this()
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the list of child components in the SampleEntity
        /// </summary>
        [Contained]
        public IEnumerable<SampleComponent> ChildComponents { get; set; }
    }
}