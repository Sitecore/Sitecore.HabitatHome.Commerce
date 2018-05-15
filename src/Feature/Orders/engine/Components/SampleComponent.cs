// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleComponent.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.Orders.Engine.Components
{
    /// <inheritdoc />
    /// <summary>
    /// The SampleComponent.
    /// </summary>
    public class SampleComponent : Component
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; set; }
    }
}