// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleEntity.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Components;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// SampleEntity model.
    /// </summary>
    public class SampleEntity : CommerceEntity
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.HabitatHome.Feature.Wishlists.Engine.Entities.SampleEntity" /> class.
        /// </summary>
        public SampleEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.HabitatHome.Feature.Wishlists.Engine.Entities.SampleEntity" /> class. 
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