// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Demo.HabitatHome.Wishlists
{
    using Sitecore.Commerce.Core;
    
    /// <inheritdoc />
    /// <summary>
    /// Defines a model
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Model" />
    public class SampleModel : Model
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Demo.HabitatHome.Wishlists.SampleModel" /> class.
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
