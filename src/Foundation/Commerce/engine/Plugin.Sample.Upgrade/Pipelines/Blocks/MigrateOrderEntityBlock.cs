// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateOrderEntityBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Pipelines;

    using System;
    using System.Linq;
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
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.MigrateOrderEntityBlock)]
    public class MigrateOrderEntityBlock : PipelineBlock<CommerceEntity, CommerceEntity, CommercePipelineExecutionContext>
    {
        private readonly FindEntityCommand _findEntityCommand;
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateOrderEntityBlock" /> class.
        /// </summary>
        /// <param name="findEntityCommand">The find entity command.</param>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        public MigrateOrderEntityBlock(FindEntityCommand findEntityCommand, IPersistEntityPipeline persistEntityPipeline)
        {
            this._findEntityCommand = findEntityCommand;
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

            if (!(arg is Order))
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

            Guid id;
            if (!Guid.TryParse(arg.Id, out id))
            {
                context.Logger.LogInformation($"{this.Name} - Invalid Order Id:{arg.Id}");
                return arg;
            }
          
            arg.Id = $"{CommerceEntity.IdPrefix<Order>()}{id:N}";

            var targetOrder = await _findEntityCommand.Process(context.CommerceContext, typeof(Order), arg.Id);
            if (targetOrder != null)
            {
                arg.IsPersisted = true;
                arg.Version = targetOrder.Version;
                return arg;
            }

            if (arg.HasComponent<ContactComponent>())
            {
                var indexByCustomerId = new EntityIndex
                {
                    Id = $"{EntityIndex.IndexPrefix<Order>("Id")}{arg.Id}",
                    IndexKey = arg.GetComponent<ContactComponent>()?.CustomerId,
                    EntityId = arg.Id
                };

                if (!migrationPolicy.ReviewOnly)
                {
                    await this._persistEntityPipeline.Run(new PersistEntityArgument(indexByCustomerId), context);
                }
            }

            var order = arg as Order;
            var indexByConfirmationId = new EntityIndex
            {
                Id = $"{EntityIndex.IndexPrefix<Order>("Id")}{order?.OrderConfirmationId}",
                IndexKey = order?.OrderConfirmationId,
                EntityId = arg.Id
            };

            if (!migrationPolicy.ReviewOnly)
            {
                await this._persistEntityPipeline.Run(new PersistEntityArgument(indexByConfirmationId), context);
            }

            return arg;
        }        
    }
}
