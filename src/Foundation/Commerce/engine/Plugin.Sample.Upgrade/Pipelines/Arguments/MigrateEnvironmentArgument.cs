// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateEnvironmentArgument.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>----------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using System;

    /// <summary>
    /// Defines the migrate environment argument
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.PipelineArgument" />
    public class MigrateEnvironmentArgument : PipelineArgument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateEnvironmentArgument" /> class.
        /// </summary>
        /// <param name="sourceEnvironment">The source environment.</param>
        /// <param name="newArtifactStoreId">The new artifact store identifier.</param>
        /// <param name="newEnvironmentName">New name of the environment.</param>
        public MigrateEnvironmentArgument(CommerceEnvironment sourceEnvironment, Guid newArtifactStoreId, string newEnvironmentName)           
        {
            Condition.Requires(sourceEnvironment).IsNotNull("The sourceEnvironment can not be null");
            Condition.Requires(newEnvironmentName).IsNotNullOrEmpty("The newEnvironmentName can not be null or empty");

            this.SourceEnvironment = sourceEnvironment;
            this.NewArtifactStoreId = newArtifactStoreId;
            this.NewEnvironmentName = newEnvironmentName;
        }

        /// <summary>
        /// Gets or sets the source environment.
        /// </summary>
        /// <value>
        /// The source environment.
        /// </value>
        public CommerceEnvironment SourceEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the new artifact store identifier.
        /// </summary>
        /// <value>
        /// The new artifact store identifier.
        /// </value>
        public Guid NewArtifactStoreId { get; set; }

        /// <summary>
        /// Gets or sets the new name of the environment.
        /// </summary>
        /// <value>
        /// The new name of the new environment.
        /// </value>
        public string NewEnvironmentName { get; set; }
    }
}
