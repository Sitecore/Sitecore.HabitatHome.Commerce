// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Defines a model
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Model" />
    public class SampleModel : Model
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.HabitatHome.Feature.NearestStore.Engine.Models.SampleModel" /> class.
        /// </summary>
        public SampleModel()
        {
            this.Id = string.Empty;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; private set; }
    }
}
