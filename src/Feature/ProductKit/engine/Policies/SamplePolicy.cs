// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplePolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Feature.ProductKit.Engine.Policies
{
    /// <inheritdoc />
    /// <summary>
    /// Defines a policy
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class SamplePolicy : Policy
    {
        /// <summary>
        /// Gets or sets the sample entity display.
        /// </summary>
        /// <value>
        /// The sample entity display.
        /// </value>
        public string SampleEntityDisplay { get; set; }
    }
}
