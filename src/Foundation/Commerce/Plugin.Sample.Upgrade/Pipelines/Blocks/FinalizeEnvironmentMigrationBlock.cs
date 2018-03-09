// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalizeEnvironmentMigrationBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a block which finalize the environment migration
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.CommerceEnvironment, System.Boolean,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.FinalizeEnvironmentMigrationBlock)]
    public class FinalizeEnvironmentMigrationBlock : PipelineBlock<CommerceEnvironment, bool, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="PersistEntityArgument"/></returns>
        public override Task<bool> Run(CommerceEnvironment arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The argument can not be null");

            if (context.CommerceContext.GetMessages().Any(m =>
                m.Code.Equals(context.CommerceContext.GetPolicy<KnownResultCodes>().Error,
                    StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}