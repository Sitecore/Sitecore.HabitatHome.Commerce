// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateGiftCardBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Entitlements;
    using Sitecore.Commerce.Plugin.GiftCards;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Pipelines;

    using System.Collections.Generic;
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
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.MigrateGiftCardBlock)]
    public class MigrateGiftCardBlock : PipelineBlock<CommerceEntity, CommerceEntity, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateGiftCardBlock"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        public MigrateGiftCardBlock(IPersistEntityPipeline persistEntityPipeline)
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

            if (!(arg is GiftCard))
            {
                return arg;
            }

            var giftCard = arg as GiftCard;
           
            var migrationPolicy = context.CommerceContext?.GetPolicy<MigrationPolicy>();
            if (migrationPolicy == null)
            {
                await context.CommerceContext.AddMessage(
                    context.CommerceContext.GetPolicy<KnownResultCodes>().Error,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "MigrationPolicy" },
                    $"{this.GetType()}. Missing MigrationPolicy");
                return giftCard;
            }

            if (giftCard.Order != null)
            {
                var orderId = $"{CommerceEntity.IdPrefix<Order>()}{giftCard.Order.EntityTarget}";
                giftCard.Order = new EntityReference(orderId);
            }

            giftCard.SetComponent(new ListMembershipsComponent
            {
                Memberships = new List<string>
                {
                    $"{CommerceEntity.ListName<Entitlement>()}",
                    $"{CommerceEntity.ListName<GiftCard>()}"
                }
            });

            if (giftCard.IsPersisted)
            {
                return giftCard;
            }

            var indexByGiftCardCode = new EntityIndex
            {
                Id = $"{EntityIndex.IndexPrefix<GiftCard>("Id")}{giftCard.GiftCardCode}",
                IndexKey = giftCard.GiftCardCode,
                EntityId = giftCard.Id
            };

            if (!migrationPolicy.ReviewOnly)
            {
                await this._persistEntityPipeline.Run(new PersistEntityArgument(indexByGiftCardCode), context);
            }

            return giftCard;
        }        
    }
}
