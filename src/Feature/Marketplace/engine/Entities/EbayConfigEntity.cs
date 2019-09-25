// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleEntity.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// SampleEntity model.
    /// </summary>
    public class EbayConfigEntity : CommerceEntity
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Sample.SampleEntity" /> class.
        /// </summary>
        public EbayConfigEntity()
        {
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Sample.SampleEntity" /> class. 
        /// Public Constructor
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public EbayConfigEntity(string id) : this()
        {
            this.Id = id;
        }

        ///// <summary>
        ///// Gets or sets the list of child components in the SampleEntity
        ///// </summary>
        //[Contained]
        //public IEnumerable<EbayItemComponent> ChildComponents { get; set; }
    }
}