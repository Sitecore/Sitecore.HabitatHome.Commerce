// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSourceEnvironmentBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the get source environment block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.PipelineBlock{Plugin.Sample.Upgrade.MigrateEnvironmentArgument,
    ///         Sitecore.Commerce.Core.CommerceEnvironment, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.GetSourceEnvironmentBlock)]
    public class GetSourceEnvironmentBlock : PipelineBlock<MigrateEnvironmentArgument, CommerceEnvironment, CommercePipelineExecutionContext>
    {
        private readonly IEntityMigrationPipeline _entityMigrationPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSourceEnvironmentBlock" /> class.
        /// </summary>
        /// <param name="entityMigrationPipeline">The entity migration pipeline.</param>
        public GetSourceEnvironmentBlock(IEntityMigrationPipeline entityMigrationPipeline)
        {
            this._entityMigrationPipeline = entityMigrationPipeline;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<CommerceEnvironment> Run(MigrateEnvironmentArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: the argument cannot be null.");

            context.Logger.LogInformation($"{this.Name} - Run IEntityMigrationPipeline:{arg.SourceEnvironment.Name}");

            context.CommerceContext.AddUniqueObjectByType(arg);           
            context.CommerceContext.Environment = arg.SourceEnvironment;
            
            CommerceEnvironment environment; 
           
            try
            {
                environment = await this._entityMigrationPipeline.Run(
                    new FindEntityArgument(typeof(CommerceEnvironment), 
                    arg.SourceEnvironment.Id)
                    { ShouldCreate = false }, 
                    context) as CommerceEnvironment;
                if (environment == null)
                {
                    context.Abort(
                        await context.CommerceContext.AddMessage(
                            context.GetPolicy<KnownResultCodes>().Error,
                            "EntityNotFound",
                            new object[] { arg.SourceEnvironment.ArtifactStoreId, arg.SourceEnvironment.Id },
                            $"Source environment ArtifactStoreId={arg.SourceEnvironment.ArtifactStoreId}, Id={arg.SourceEnvironment.Id} was not found."),
                        context);
                    return null;
                }

                // update ArtifactStoreId to get lists
                arg.SourceEnvironment.ArtifactStoreId = environment.ArtifactStoreId;
                environment.ArtifactStoreId = arg.NewArtifactStoreId;
                environment.Name = arg.NewEnvironmentName;
                environment.Id = $"{CommerceEntity.IdPrefix<CommerceEnvironment>()}{arg.NewEnvironmentName}";
                environment.IsPersisted = false;
                environment.Version = 1;

                var sqlPoliciesCollection = context.CommerceContext?.GetObjects<List<KeyValuePair<string, EntityStoreSqlPolicy>>>()?.FirstOrDefault();
                if (sqlPoliciesCollection != null)
                {
                    var sourceGlobal = sqlPoliciesCollection.FirstOrDefault(p =>
                        p.Key.Equals("SourceGlobal", StringComparison.OrdinalIgnoreCase)).Value;
                    var sqlPolicy = new EntityStoreSqlPolicy
                    {
                        Server = sourceGlobal.Server,
                        Database = environment.GetPolicy<EntityStoreSqlPolicy>().Database,
                        TrustedConnection = sourceGlobal.TrustedConnection,
                        ConnectTimeout = 120000,
                        CleanEnvironmentCommandTimeout = 120000,
                        UserName = sourceGlobal.UserName,
                        Password = sourceGlobal.Password,
                        AdditionalParameters = sourceGlobal.AdditionalParameters
                    };
                    sqlPoliciesCollection.Add(
                        new KeyValuePair<string, EntityStoreSqlPolicy>("SourceShared", sqlPolicy));
                }

                context.CommerceContext.AddUniqueObjectByType(arg);
            }
            catch (Exception ex)
            {
                await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        this.Name,
                        new object[] { ex },
                        $"{this.Name}.Exception: {ex.Message}");
                context.CommerceContext.Environment = context.CommerceContext.GlobalEnvironment;
                context.CommerceContext.LogException($"{this.Name}. Exception when getting {arg.SourceEnvironment.Name}", ex);
                return arg.SourceEnvironment;
            }

            context.CommerceContext.Environment = context.CommerceContext.GlobalEnvironment;
            return environment;
        }        
    }
}
