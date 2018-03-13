// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectEnvironmentPoliciesBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Caching;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the inject environment policies block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.PipelineBlock{Sitecore.Commerce.Core.CommerceEnvironment,
    ///         Sitecore.Commerce.Core.CommerceEnvironment, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.InjectEnvironmentPoliciesBlock)]
    public class InjectEnvironmentPoliciesBlock : PipelineBlock<CommerceEnvironment, CommerceEnvironment, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override Task<CommerceEnvironment> Run(CommerceEnvironment arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: the argument cannot be null.");
            context.Logger.LogInformation($"{this.Name} - Injecting policies to {arg.Name} environment");

            var migrateEnvironmentArgument = context.CommerceContext?.GetObjects<MigrateEnvironmentArgument>()?.FirstOrDefault();
            var sqlPoliciesCollection = context.CommerceContext?.GetObjects<List<KeyValuePair<string, EntityStoreSqlPolicy>>>()?.FirstOrDefault();

            if (migrateEnvironmentArgument == null || sqlPoliciesCollection == null)
            {
                return null;
            }

            var sourceEnvironment = migrateEnvironmentArgument?.SourceEnvironment;
            foreach (var policy in sourceEnvironment.GetPolicies<PolicySetPolicy>())
            {
                arg.Policies.Add(policy);
            }

            foreach (var policy in sourceEnvironment.GetPolicies<ValidationPolicy>())
            {
                arg.Policies.Add(policy);
            }            

            sourceEnvironment.SetPolicy(sqlPoliciesCollection.FirstOrDefault(p => p.Key.Equals("SourceShared", StringComparison.OrdinalIgnoreCase)).Value);
            arg.SetPolicy(sqlPoliciesCollection.FirstOrDefault(p => p.Key.Equals("DestinationShared", StringComparison.OrdinalIgnoreCase)).Value);                                 

            if (sourceEnvironment.HasPolicy<CustomersRemovePolicy>())
            {
                arg.Policies.Add(sourceEnvironment.GetPolicy<CustomersRemovePolicy>());
            }

            if (sourceEnvironment.HasPolicy<MinionPolicy>())
            {
                arg.Policies.Add(sourceEnvironment.GetPolicy<MinionPolicy>());
            }

            if (sourceEnvironment.HasPolicy<GlobalCurrencyPolicy>())
            {
                arg.Policies.Add(sourceEnvironment.GetPolicy<GlobalCurrencyPolicy>());
            }

            if (sourceEnvironment.HasPolicy<VariationPropertyPolicy>())
            {
                arg.Policies.Add(sourceEnvironment.GetPolicy<VariationPropertyPolicy>());
            }

            var shardingPolicy = context.CommerceContext?.GetObjects<EntityShardingPolicy>()?.FirstOrDefault();
            if (shardingPolicy != null)
            {
                arg.Policies.Add(shardingPolicy);
            }

            arg.Policies.Add(new EntityMemoryCachingPolicy { EntityFullName = "Sitecore.Commerce.Plugin.Customers.Customer", AllowCaching = false });
            arg.Policies.Add(new EntityMemoryCachingPolicy { EntityFullName = "Sitecore.Commerce.Core.LocalizationEntity", AllowCaching = false });

            return Task.FromResult(arg);
        }        
    }
}
