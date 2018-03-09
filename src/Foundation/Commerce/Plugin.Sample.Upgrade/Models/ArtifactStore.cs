// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArtifactStore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>----------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;

    /// <summary>
    /// Defines the ArtifactStore module 
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Model" />
    public class ArtifactStore : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactStore"/> class.
        /// </summary>
        /// <param name="artifactStoreId">The artifact store identifier.</param>
        public ArtifactStore(string artifactStoreId)
        {
            Condition.Requires(artifactStoreId).IsNotNullOrEmpty("The artifactStoreId can not be null or empty");
            this.ArtifactStoreId = artifactStoreId;
        }

        /// <summary>
        /// Gets or sets the artifact store identifier.
        /// </summary>
        /// <value>
        /// The artifact store identifier.
        /// </value>
        public string ArtifactStoreId { get; set; }
    }
}
