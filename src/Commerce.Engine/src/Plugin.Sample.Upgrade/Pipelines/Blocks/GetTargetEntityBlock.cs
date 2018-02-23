// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetTargetEntityBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the get target entity block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.PipelineBlock{ Sitecore.Commerce.Core.FindEntityArgument,
    ///         Sitecore.Commerce.Core.FindEntityArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.GetTargetEntityBlock)]
    public class GetTargetEntityBlock : PipelineBlock<FindEntityArgument, FindEntityArgument, CommercePipelineExecutionContext>
    {
        private readonly FindEntityCommand _findEntityCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTargetEntityBlock" /> class.
        /// </summary>
        /// <param name="findEntityCommand">The find entity command.</param>
        public GetTargetEntityBlock(FindEntityCommand findEntityCommand)
        {
            this._findEntityCommand = findEntityCommand;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<FindEntityArgument> Run(FindEntityArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: the argument cannot be null.");

            context.Logger.LogInformation($"{this.Name} - Run IEntityMigrationPipeline:{arg.EntityId}");
            var targetEntity = await _findEntityCommand.Process(context.CommerceContext, typeof(CommerceEntity), arg.EntityId);

            arg.Entity = targetEntity;

            return arg;
        }        
    }
}
