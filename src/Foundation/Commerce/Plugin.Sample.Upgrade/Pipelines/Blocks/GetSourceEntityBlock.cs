// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSourceEntityBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the get source entity block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.PipelineBlock{ Sitecore.Commerce.Core.FindEntityArgument,
    ///         Sitecore.Commerce.Core.CommerceEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.GetSourceEntityBlock)]
    public class GetSourceEntityBlock : PipelineBlock<FindEntityArgument, CommerceEntity, CommercePipelineExecutionContext>
    {
        private readonly IEntityMigrationPipeline _entityMigrationPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSourceEntityBlock" /> class.
        /// </summary>
        /// <param name="entityMigrationPipeline">The entity migration pipeline.</param>
        public GetSourceEntityBlock(IEntityMigrationPipeline entityMigrationPipeline)
        {
            this._entityMigrationPipeline = entityMigrationPipeline;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<CommerceEntity> Run(FindEntityArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: the argument cannot be null.");

            var allEntitiesList = context.CommerceContext?.GetObjects<List<string>>()?.FirstOrDefault();
            var migrateEnvironmentArgument = context.CommerceContext?.GetObjects<MigrateEnvironmentArgument>()?.FirstOrDefault();
            if (migrateEnvironmentArgument == null || allEntitiesList == null)
            {
                return null;
            }

            var newEnvironment = context.CommerceContext.Environment;
            context.CommerceContext.Environment = migrateEnvironmentArgument.SourceEnvironment;

            CommerceEntity entity; 
           
            try
            {
                var entityId = allEntitiesList.FirstOrDefault(id => id.IndexOf(arg.EntityId, StringComparison.OrdinalIgnoreCase) > 0);
                if (!string.IsNullOrEmpty(entityId) && arg.Entity != null)
                {
                    entity = arg.Entity;
                }
                else
                {
                    entity = await this._entityMigrationPipeline.Run(
                        new FindEntityArgument(typeof(CommerceEntity),
                        arg.EntityId)
                        { ShouldCreate = false }, 
                        context);

                    if (entity != null)
                    {
                        entity.IsPersisted = arg.Entity != null;
                        allEntitiesList.Add(arg.EntityId);
                        if (arg.Entity != null)
                        {
                            entity.Version = arg.Entity.Version;
                        }
                    }
                }

                // we are not migrating EntityIndex as it will be created 
                if (entity != null && entity is EntityIndex)
                {
                    entity = null;
                }
            }
            catch (Exception ex)
            {
                await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        this.Name,
                        new object[] { ex },
                        $"{this.Name}.Exception: {ex.Message}");
                context.CommerceContext.Environment = newEnvironment;
                context.CommerceContext.LogException($"{this.Name}.Exception getting source {arg.EntityId}", ex);
                return arg.Entity;
            }

            context.CommerceContext.Environment = newEnvironment;
            return entity;
        }        
    }
}
