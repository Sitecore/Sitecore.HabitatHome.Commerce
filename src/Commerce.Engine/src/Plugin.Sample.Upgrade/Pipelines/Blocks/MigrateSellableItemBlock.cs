// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateSellableItemBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Catalog;
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
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.MigrateSellableItemBlock)]
    public class MigrateSellableItemBlock : PipelineBlock<CommerceEntity, CommerceEntity, CommercePipelineExecutionContext>
    {
        private readonly FindEntityCommand _findEntityCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateSellableItemBlock"/> class.
        /// </summary>
        /// <param name="findEntityCommand">The find entity command.</param>
        public MigrateSellableItemBlock(FindEntityCommand findEntityCommand)
        {
            this._findEntityCommand = findEntityCommand;
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

            if (!(arg is SellableItem))
            {
                return arg;
            }

            var sellableItem = arg as SellableItem;
            var productId = arg.Id.Substring($"{CommerceEntity.IdPrefix<SellableItem>()}".Length);
            productId = productId.Replace("-", " ");

            sellableItem.Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{productId}";
            sellableItem.ProductId = productId;
            sellableItem.FriendlyId = productId;

            var targetSellableItem = await _findEntityCommand.Process(context.CommerceContext, typeof(SellableItem), sellableItem.Id);
            if (targetSellableItem != null)
            {
                sellableItem.IsPersisted = true;
                sellableItem.Version = targetSellableItem.Version;
            }

            return sellableItem;
        }        
    }
}
