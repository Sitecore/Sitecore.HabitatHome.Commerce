// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpgradeEnvironmentsCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>// 
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.SQL;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a get user site terms command
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Commands.CommerceCommand" />
    public class MigrateEnvironmentCommand : CommerceCommand
    {
        private readonly IMigrateEnvironmentPipeline _migrateEnvironmentPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateEnvironmentCommand" /> class.
        /// </summary>
        /// <param name="migrateEnvironmentPipeline">The migrate environment pipeline.</param>
        /// <param name="findEntityPipeline">The find entity pipeline.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public MigrateEnvironmentCommand(
            IMigrateEnvironmentPipeline migrateEnvironmentPipeline,
            IFindEntityPipeline findEntityPipeline,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._migrateEnvironmentPipeline = migrateEnvironmentPipeline;
            this._findEntityPipeline = findEntityPipeline;
        }

        /// <summary>
        /// Processes the specified commerce context.
        /// </summary>
        /// <param name="commerceContext">The commerce context.</param>
        /// <param name="sourceEnvironmentName">Name of the source environment.</param>
        /// <param name="newEnvironmentName">New name of the environment.</param>
        /// <param name="newArtifactStoreId">The new artifact store identifier.</param>
        /// <returns>
        /// User site terms
        /// </returns>
        public virtual async Task<bool> Process(CommerceContext commerceContext, string sourceEnvironmentName, string newEnvironmentName, Guid newArtifactStoreId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var migrationSqlPolicy = commerceContext.GetPolicy<MigrationSqlPolicy>();
                if (migrationSqlPolicy == null)
                {
                    await commerceContext.AddMessage(
                        commerceContext.GetPolicy<KnownResultCodes>().Error,
                        "InvalidOrMissingPropertyValue",
                        new object[] { "MigrationSqlPolicy" },
                        $"{this.GetType()}. Missing MigrationSqlPolicy");
                    return false;
                }

                if (string.IsNullOrEmpty(migrationSqlPolicy.SourceStoreSqlPolicy.Server))
                {
                    await commerceContext.AddMessage(
                        commerceContext.GetPolicy<KnownResultCodes>().Error,
                        "InvalidOrMissingPropertyValue",
                        new object[] { "MigrationSqlPolicy" },
                        $"{this.GetType()}. Empty server name in the MigrationSqlPolicy");
                    return false;
                }

                var migrationPolicy = commerceContext.GetPolicy<MigrationPolicy>();
                if (migrationPolicy == null)
                {
                    await commerceContext.AddMessage(
                        commerceContext.GetPolicy<KnownResultCodes>().Error,
                        "InvalidOrMissingPropertyValue",
                        new object[] { "MigrationPolicy" },
                        $"{this.GetType()}. Missing MigrationPolicy");
                    return false;
                }

                Guid id;           
                if (!Guid.TryParse(migrationSqlPolicy.ArtifactStoreId, out id))
                {
                    await commerceContext.AddMessage(
                        commerceContext.GetPolicy<KnownResultCodes>().Error,
                        "InvalidOrMissingPropertyValue",
                        new object[] { "MigrationSqlPolicy.ArtifactStoreId" },
                        "MigrationSqlPolicy. Invalid ArtifactStoreId");
                    return false;
                }

                var context = commerceContext.GetPipelineContextOptions();

                var findArg = new FindEntityArgument(typeof(CommerceEnvironment), $"{CommerceEntity.IdPrefix<CommerceEnvironment>()}{newEnvironmentName}");
                var findResult = await this._findEntityPipeline.Run(findArg, context);
                var newEnvironment = findResult as CommerceEnvironment;
                if (newEnvironment != null)
                {
                    await commerceContext.AddMessage(
                        commerceContext.GetPolicy<KnownResultCodes>().Error,
                        "InvalidOrMissingPropertyValue",
                        new object[] { newEnvironmentName },
                        $"Environment {newEnvironmentName} already exists.");
                    return false;
                }

                findArg = new FindEntityArgument(typeof(CommerceEnvironment), $"{CommerceEntity.IdPrefix<CommerceEnvironment>()}{migrationPolicy.DefaultEnvironmentName}");
                findResult = await this._findEntityPipeline.Run(findArg, context);

                var sourceEnvironment = findResult as CommerceEnvironment;
                if (sourceEnvironment == null)
                {
                    await commerceContext.AddMessage(
                        commerceContext.GetPolicy<KnownResultCodes>().Error,
                        "EntityNotFound",
                        new object[] { migrationPolicy.DefaultEnvironmentName },
                        $"Entity {migrationPolicy.DefaultEnvironmentName} was not found.");
                    return false;
                }

                if (sourceEnvironment.HasPolicy<EntityShardingPolicy>())
                {
                    commerceContext.AddUniqueObjectByType(sourceEnvironment.GetPolicy<EntityShardingPolicy>());
                    sourceEnvironment.RemovePolicy(typeof(EntityShardingPolicy));
                }

                // set sql policies
                var sqlPoliciesCollection = new List<KeyValuePair<string, EntityStoreSqlPolicy>>
                {
                    new KeyValuePair<string, EntityStoreSqlPolicy>("SourceGlobal", migrationSqlPolicy.SourceStoreSqlPolicy)
                };

                // Get sql Policy set
                var ps = await this._findEntityPipeline.Run(new FindEntityArgument(typeof(PolicySet), $"{CommerceEntity.IdPrefix<PolicySet>()}{migrationPolicy.SqlPolicySetName}"), context);
                if (ps == null)
                {
                    context.CommerceContext.Logger.LogError($"PolicySet {migrationPolicy.SqlPolicySetName} was not found.");
                }
                else
                {
                    sqlPoliciesCollection.Add(new KeyValuePair<string, EntityStoreSqlPolicy>("DestinationShared", ps.GetPolicy<EntityStoreSqlPolicy>()));
                }

                commerceContext.AddUniqueObjectByType(sqlPoliciesCollection);

                sourceEnvironment.Name = sourceEnvironmentName;
                sourceEnvironment.ArtifactStoreId = id;
                sourceEnvironment.Id = $"{CommerceEntity.IdPrefix<CommerceEnvironment>()}{sourceEnvironmentName}";            
                sourceEnvironment.SetPolicy(migrationSqlPolicy.SourceStoreSqlPolicy);            

                var migrateEnvironmentArgument = new MigrateEnvironmentArgument(sourceEnvironment, newArtifactStoreId, newEnvironmentName);                     

                var result = await this._migrateEnvironmentPipeline.Run(migrateEnvironmentArgument, context);
                return result;
            }
        }
    }
}
