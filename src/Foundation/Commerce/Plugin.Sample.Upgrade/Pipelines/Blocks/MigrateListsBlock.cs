// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateListsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Journaling;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the get course environment block.
    /// </summary>
    /// <seealso>
    ///   <cref>
    ///     Sitecore.Commerce.Core.PipelineBlock{Sitecore.Commerce.Core.CommerceEnvironment,
    ///     Sitecore.Commerce.Core.CommerceEnvironment, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///   </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.MigrateListsBlock)]
    public class MigrateListsBlock : PipelineBlock<CommerceEnvironment, CommerceEnvironment, CommercePipelineExecutionContext>
    {
        private readonly IGetListsNamesPipeline _getListsNamesPipeline;
        private readonly IMigrateListPipeline _migrateListPipeline;
        private readonly IStartEnvironmentPipeline _startEnvironmentPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateListsBlock" /> class.
        /// </summary>
        /// <param name="getListsNamesPipeline">The get lists names pipeline.</param>
        /// <param name="migrateListPipeline">The migrate list pipeline.</param>
        /// <param name="startEnvironmentPipeline">The start environment pipeline.</param>
        public MigrateListsBlock(
            IGetListsNamesPipeline getListsNamesPipeline,
            IMigrateListPipeline migrateListPipeline, 
            IStartEnvironmentPipeline startEnvironmentPipeline)
        {
            this._getListsNamesPipeline = getListsNamesPipeline;
            this._migrateListPipeline = migrateListPipeline;
            this._startEnvironmentPipeline = startEnvironmentPipeline;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>Migrated environment</returns>
        public override async Task<CommerceEnvironment> Run(CommerceEnvironment arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: the argument cannot be null.");

            context.Logger.LogInformation($"{this.Name} - Start lists migration");

            var migrationPolicy = context.CommerceContext?.GetPolicy<MigrationPolicy>();
            if (migrationPolicy == null)
            {
                await context.CommerceContext.AddMessage(
                    context.CommerceContext.GetPolicy<KnownResultCodes>().Error,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "MigrationPolicy" },
                    $"{this.GetType()}. Missing MigrationPolicy");
                return arg;
            }

            context.CommerceContext.AddUniqueObjectByType(new List<string>());
            
            // Start Environment to initilize policySets
            var environment = await this._startEnvironmentPipeline.Run(arg.Name, context);

            // not adding new JournalEntiry during the migration
            foreach (var policy in environment.GetPolicies<EntityJournalingPolicy>())
            {
                arg.Policies.Remove(policy);
            }

            // Allow to migrate enitities with not alphanumeric names
            foreach (var policy in environment.GetPolicies<ValidationPolicy>())
            {
                var models = new List<Model>();
                foreach (ValidationAttributes model in policy.Models)
                {
                    if (!model.RegexValidatorErrorCode.Equals("AlphanumericOnly_NameValidationError", StringComparison.OrdinalIgnoreCase))
                    {
                        models.Add(model);
                    }
                }

                policy.Models = models;
            }

            // get full list of lists
            var listsOfLists = new Dictionary<string, int?>();
            var listNamePolicy = context.CommerceContext.GetPolicy<ListNamePolicy>();
            var migrateEnvironmentArgument = context.CommerceContext?.GetObjects<MigrateEnvironmentArgument>()?.FirstOrDefault();
            context.CommerceContext.Environment = migrateEnvironmentArgument?.SourceEnvironment;
            foreach (var list in migrationPolicy.ListsToMigrate)
            {
                int maxCount = list.Value ?? int.MaxValue;
                if (maxCount == 0)
                {
                    continue;
                }

                if (list.Key.IndexOf('*') < 0)
                {
                    listsOfLists.Add(list.Key, list.Value);
                }
                else
                {
                    List<string> listNames = await this._getListsNamesPipeline.Run(list.Key.Replace("*", "%"), context);
                    if (listNames != null && listNames.Any())
                    {
                        foreach (var name in listNames)
                        {
                            var listMembership = name.Substring($"{listNamePolicy.Prefix}{listNamePolicy.Separator}".Length);
                            listMembership = listMembership.Replace($"{listNamePolicy.Separator}{listNamePolicy.Suffix}", string.Empty);
                            listsOfLists.Add(listMembership, list.Value);
                        }
                    }
                }
            }

            context.CommerceContext.Environment = environment;

            foreach (var list in listsOfLists)
            {
                context.Logger.LogInformation($"{this.Name} - Running list migration: {list.Key}");
                Task.Run(() => this._migrateListPipeline.Run(new MigrateListArgument(list.Key) { MaxCount = list.Value ?? int.MaxValue }, context)).Wait();
                context.Logger.LogInformation($"{this.Name} - Done with list migration: {list.Key}");
            }

            context.Logger.LogInformation($"{this.Name} - Done with all lists migration");
            return arg;
        }        
    }
}
