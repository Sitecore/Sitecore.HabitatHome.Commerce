// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistMigratedEntityBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    using System.Threading.Tasks;

    /// <summary>
    /// Defines the get source entity block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.PipelineBlock{ Sitecore.Commerce.Core.CommerceEntity,
    ///         Sitecore.Commerce.Core.CommerceEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.PersistMigratedEntityBlock)]
    public class PersistMigratedEntityBlock : PipelineBlock<CommerceEntity, CommerceEntity, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSourceEntityBlock" /> class.
        /// </summary>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        public PersistMigratedEntityBlock(IPersistEntityPipeline persistEntityPipeline)
        {
            this._persistEntityPipeline = persistEntityPipeline;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<CommerceEntity> Run(CommerceEntity arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return arg;
            }

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

            if (migrationPolicy.ReviewOnly)
            {
                return arg;
            }

            context.Logger.LogInformation($"{this.Name} - Run IPersistEntityPipeline:{arg.Id}");
            await this._persistEntityPipeline.Run(new PersistEntityArgument(arg), context);
            return arg;
        }        
    }
}
