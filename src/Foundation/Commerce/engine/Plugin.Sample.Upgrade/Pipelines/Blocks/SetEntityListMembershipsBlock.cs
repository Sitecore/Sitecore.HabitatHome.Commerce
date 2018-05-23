// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetEntityListMembershipsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the set CommerceEntity list memberships block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.CommerceEntity,
    ///         Sitecore.Commerce.Core.CommerceEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.SetEntityListMembershipsBlock)]
    public class SetEntityListMembershipsBlock : PipelineBlock<CommerceEntity, CommerceEntity, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="CommerceEntity"/></returns>
        public override Task<CommerceEntity> Run(CommerceEntity arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return Task.FromResult(arg);
            }

            var migrateListArgument = context.CommerceContext?.GetObjects<MigrateListArgument>()?.FirstOrDefault();
            if (migrateListArgument == null)
            {
                return Task.FromResult(arg);
            }

            if (arg.HasComponent<ListMembershipsComponent>())
            {
                var listMemberships = arg.GetComponent<ListMembershipsComponent>().Memberships;
                if (!listMemberships.Contains(migrateListArgument.ListName, StringComparer.OrdinalIgnoreCase))
                {
                    listMemberships.Add(migrateListArgument.ListName);
                }
            }
            else
            {
                arg.SetComponent(new ListMembershipsComponent
                {
                    Memberships = new List<string>
                    {
                        migrateListArgument.ListName
                    }
                });
            }

            return Task.FromResult(arg);
        }
    }
}
