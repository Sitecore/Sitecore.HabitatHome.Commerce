// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateListBlock.cs" company="Sitecore Corporation">
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
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the get course environment block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.PipelineBlock{Plugin.Sample.Upgrade.MigrateListArgument,
    ///         System.Boolean, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.MigrateListBlock)]
    public class MigrateListBlock : PipelineBlock<MigrateListArgument, bool, CommercePipelineExecutionContext>
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;
        private readonly IMigrateEntityMetadataPipeline _migrateEntityMetadataPipeline;
        private readonly GetListCountCommand _getListCountCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSourceEnvironmentBlock" /> class.
        /// </summary>
        /// <param name="findEntitiesInListPipeline">The find entities in list pipeline.</param>
        /// <param name="migrateEntityMetadataPipeline">The migrate entity metadata pipeline.</param>
        /// <param name="getListCountCommand">The get list count command.</param>
        public MigrateListBlock(
            IFindEntitiesInListPipeline findEntitiesInListPipeline, 
            IMigrateEntityMetadataPipeline migrateEntityMetadataPipeline,
            GetListCountCommand getListCountCommand)
        {
            this._findEntitiesInListPipeline = findEntitiesInListPipeline;
            this._migrateEntityMetadataPipeline = migrateEntityMetadataPipeline;
            this._getListCountCommand = getListCountCommand;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<bool> Run(MigrateListArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: the argument cannot be null.");
            context.Logger.LogInformation($"{this.Name} - Migrating the list:{arg.ListName}");

            var count = await this._getListCountCommand.Process(context.CommerceContext, arg.ListName);
            if (count > 0)
            {
                context.Logger.LogInformation($"{this.Name} - List {arg.ListName} was already migrated. Skipping it.");
                return true;
            }

            var migrateEnvironmentArgument = context.CommerceContext?.GetObjects<MigrateEnvironmentArgument>()?.FirstOrDefault();

            context.CommerceContext.AddUniqueObjectByType(arg);
            var newEnvironment = context.CommerceContext.Environment;
            var result = true;

            try
            {               
                context.CommerceContext.Environment = migrateEnvironmentArgument?.SourceEnvironment;
                var entitiesArgument = await _findEntitiesInListPipeline.Run(
                    new FindEntitiesInListArgument(
                    typeof(CommerceEntity),
                    arg.ListName,
                    0,
                    arg.MaxCount)
                    {
                        LoadEntities = false
                    },
                    context);

                context.CommerceContext.Environment = newEnvironment;
                foreach (var id in entitiesArgument.IdList)
                {
                    var migratedEntity = await this._migrateEntityMetadataPipeline.Run(new FindEntityArgument(typeof(CommerceEntity), id), context);
                    if (migratedEntity == null)
                    {
                        context.Logger.LogInformation($"{this.Name} - Entity {id} was not migrated.");
                        result = false;
                    }                    
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
                context.CommerceContext.LogException($"{this.Name}. Exception when migrating the list {arg.ListName}", ex);
                return false;
            }
            
            return result;
        }        
    }
}
