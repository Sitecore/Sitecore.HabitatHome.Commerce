// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleComponent.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Plugin.Sample
{
    using Sitecore.Commerce.Core;

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