// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Sitecore.Commerce.Core;  
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Commerce.Plugin.ManagedLists;

    /// <summary>
    /// The Habitat configure class.
    /// </summary>
    /// <seealso cref="Sitecore.Framework.Configuration.IConfigureSitecore" />
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);
            services.RegisterAllCommands(assembly);

            services.Sitecore().Pipelines(config => config
              .AddPipeline<IMigrateEnvironmentMetadataPipeline, MigrateEnvironmentMetadataPipeline>(
                 c =>
                 {
                     c.Add<GetSourceEnvironmentBlock>()
                     .Add<InjectEnvironmentPoliciesBlock>()
                     .Add<SetEnvironmentListMembershipsBlock>()
                     .Add<PersistEnvironmentBlock>()
                     .Add<ClearEnvironmentCacheBlock>();                     
                 })
               .AddPipeline<IMigrateEnvironmentPipeline, MigrateEnvironmentPipeline>(
                 c =>
                 {
                     c.Add<IMigrateEnvironmentMetadataPipeline>()
                      .Add<MigrateListsBlock>()
                      .Add<ClearEnvironmentCacheBlock>()
                      .Add<FinalizeEnvironmentMigrationBlock>();
                 })
                .AddPipeline<IMigrateListPipeline, MigrateListPipeline>(
                 c =>
                 {
                     c.Add<MigrateListBlock>();
                 })
                 .AddPipeline<IMigrateEntityMetadataPipeline, MigrateEntityMetadataPipeline>(
                 c =>
                 {
                     c.Add<GetTargetEntityBlock>()
                     .Add<GetSourceEntityBlock>()
                     .Add<MigrateOrderEntityBlock>()
                     .Add<MigrateGiftCardBlock>()
                     .Add<MigrateJournalEntryBlock>()
                     .Add<MigrateSellableItemBlock>()
                     .Add<SetEntityListMembershipsBlock>()
                     .Add<PersistMigratedEntityBlock>();                      
                 })
               .ConfigurePipeline<IConfigureOpsServiceApiPipeline>(c => { c.Add<ConfigureOpsServiceApiBlock>(); })
               .ConfigurePipeline<IEntityMigrationPipeline>(c => c.Add<PatchEnvironmentJsonBlock>().After<FindEntityJsonBlock>())
            );
        }        
    }
}