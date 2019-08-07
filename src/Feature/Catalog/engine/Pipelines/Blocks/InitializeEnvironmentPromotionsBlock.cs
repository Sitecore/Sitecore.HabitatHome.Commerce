// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeEnvironmentPromotionsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.HabitatHome.Feature.Catalog.Engine.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Plugin.Rules;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which bootstraps promotions.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(HabitatHomeConstants.Pipelines.Blocks.InitializePromotionsBlock)]
    public class InitializeEnvironmentPromotionsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IAddPromotionBookPipeline _addBookPipeline;
        private readonly IAddPromotionPipeline _addPromotionPipeline;
        private readonly IAddQualificationPipeline _addQualificationPipeline;
        private readonly IAddBenefitPipeline _addBenefitPipeline;
        private readonly IAddPublicCouponPipeline _addPublicCouponPipeline;
        private readonly IAddPromotionItemPipeline _addPromotionItemPipeline;
        private readonly IAssociateCatalogToBookPipeline _associateCatalogToBookPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeEnvironmentPromotionsBlock"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        /// <param name="addBookPipeline">The add book pipeline.</param>
        /// <param name="addPromotionPipeline">The add promotion pipeline.</param>
        /// <param name="addQualificationPipeline">The add qualification pipeline.</param>
        /// <param name="addBenefitPipeline">The add benefit pipeline.</param>
        /// <param name="addPromotionItemPipeline">The add promotion item pipeline.</param>
        /// <param name="addPublicCouponPipeline">The add public coupon pipeline.</param>
        /// <param name="associateCatalogToBookPipeline">The add public coupon pipeline.</param>
        public InitializeEnvironmentPromotionsBlock(
            IPersistEntityPipeline persistEntityPipeline,
            IAddPromotionBookPipeline addBookPipeline,
            IAddPromotionPipeline addPromotionPipeline,
            IAddQualificationPipeline addQualificationPipeline,
            IAddBenefitPipeline addBenefitPipeline,
            IAddPromotionItemPipeline addPromotionItemPipeline,
            IAddPublicCouponPipeline addPublicCouponPipeline,
            IAssociateCatalogToBookPipeline associateCatalogToBookPipeline)
        {
            this._persistEntityPipeline = persistEntityPipeline;
            this._addBookPipeline = addBookPipeline;
            this._addPromotionPipeline = addPromotionPipeline;
            this._addQualificationPipeline = addQualificationPipeline;
            this._addBenefitPipeline = addBenefitPipeline;
            this._addPromotionItemPipeline = addPromotionItemPipeline;
            this._addPublicCouponPipeline = addPublicCouponPipeline;
            this._associateCatalogToBookPipeline = associateCatalogToBookPipeline;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.Habitat.Promotions-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>()
                    .InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            context.Logger.LogInformation($"{this.Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");

            var book =
                await this._addBookPipeline.Run(
                    new AddPromotionBookArgument("Habitat_PromotionBook")
                    {
                        DisplayName = "Habitat Promotion Book",
                        Description = "This is the Habitat promotion book"
                    },
                    context).ConfigureAwait(false);

            await this.CreateCartFreeShippingPromotion(book, context).ConfigureAwait(false);
            await this.CreateCartExclusive5PctOffCouponPromotion(book, context).ConfigureAwait(false);
            await this.CreateCartExclusive5OffCouponPromotion(book, context).ConfigureAwait(false);
            await this.CreateCartExclusiveOptixCameraPromotion(book, context).ConfigureAwait(false);
            await this.CreateCart15PctOffCouponPromotion(book, context).ConfigureAwait(false);
            await this.CreateDisabledPromotion(book, context).ConfigureAwait(false);

            var date = DateTimeOffset.UtcNow;
            await this.CreateCart10PctOffCouponPromotion(book, context, date).ConfigureAwait(false);
            System.Threading.Thread.Sleep(1); //// TO ENSURE CREATING DATE IS DIFFERENT BETWEEN THESE TWO PROMOTIONS
            await this.CreateCart10OffCouponPromotion(book, context, date).ConfigureAwait(false);

            await this.CreateLineTouchScreenPromotion(book, context).ConfigureAwait(false);
            await this.CreateLineTouchScreen5OffPromotion(book, context).ConfigureAwait(false);
            await this.CreateLineExclusiveMiraLaptopPromotion(book, context).ConfigureAwait(false);
            await this.CreateLineExclusive20PctOffCouponPromotion(book, context).ConfigureAwait(false);
            await this.CreateLineExclusive20OffCouponPromotion(book, context).ConfigureAwait(false);
            await this.CreateLine5PctOffCouponPromotion(book, context).ConfigureAwait(false);
            await this.CreateLine5OffCouponPromotion(book, context).ConfigureAwait(false);
            await this.CreateLineLaptopPricePromotion(book, context).ConfigureAwait(false);
            await this.CreateBundleFitnessPromotion(book, context).ConfigureAwait(false);
            await this.CreateFreeShippingFoodieCouponPromotion(book, context).ConfigureAwait(false);
            await this.AssociateCatalogToBook(book.Name, "Habitat_Master", context).ConfigureAwait(false);

            return arg;
        }

        #region Cart's Promotions

        /// <summary>
        /// Creates the cart free shipping promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateCartFreeShippingPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "CartFreeShippingPromotion", DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1), "Free Shipping", "Free Shipping")
                    {
                        DisplayName = "Free Shipping",
                        Description = "Free shipping when Cart subtotal of $100 or more"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalCondition,
                            Name = CartsConstants.CartSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "100", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = FulfillmentConstants.CartHasFulfillmentCondition,
                            Name = FulfillmentConstants.CartHasFulfillmentCondition,
                            Properties = new List<PropertyModel>()
                        }),
                    context).ConfigureAwait(false);

            await this._addBenefitPipeline.Run(
                new PromotionActionModelArgument(
                    promotion,
                    new ActionModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        LibraryId = FulfillmentConstants.CartFreeShippingAction,
                        Name = FulfillmentConstants.CartFreeShippingAction
                    }),
                context).ConfigureAwait(false);

            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates cart exclusive 5 percent off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateCartExclusive5PctOffCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Cart5PctOffExclusiveCouponPromotion", DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddYears(1), "5% Off Cart (Exclusive Coupon)", "5% Off Cart (Exclusive Coupon)")
                    {
                        IsExclusive = true,
                        DisplayName = "5% Off Cart (Exclusive Coupon)",
                        Description = "5% off Cart with subtotal of $10 or more (Exclusive Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalCondition,
                            Name = CartsConstants.CartAnyItemSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "10", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalPercentOffAction,
                            Name = CartsConstants.CartSubtotalPercentOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "PercentOff", Value = "5", DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABRTRNEC5P"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the cart exclusive5 off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateCartExclusive5OffCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Cart5OffExclusiveCouponPromotion", DateTimeOffset.UtcNow.AddDays(-3), DateTimeOffset.UtcNow.AddYears(1), "$5 Off Cart (Exclusive Coupon)", "$5 Off Cart (Exclusive Coupon)")
                    {
                        IsExclusive = true,
                        DisplayName = "$5 Off Cart (Exclusive Coupon)",
                        Description = "$5 off Cart with subtotal of $10 or more (Exclusive Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalCondition,
                            Name = CartsConstants.CartAnyItemSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "10", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalAmountOffAction,
                            Name = CartsConstants.CartSubtotalAmountOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "AmountOff", Value = "5", DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABRTRNEC5A"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates cart exclusive optix camera promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateCartExclusiveOptixCameraPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "CartOptixCameraExclusivePromotion", DateTimeOffset.UtcNow.AddDays(-4), DateTimeOffset.UtcNow.AddYears(1), "Optix Camera 50% Off Cart (Exclusive)", "Optix Camera 50% Off Cart (Exclusive)")
                    {
                        IsExclusive = true,
                        DisplayName = "Optix Camera 50% Off Cart (Exclusive)",
                        Description = "50% off Cart when buying Optix Camera (Exclusive)"
                    },
                    context).ConfigureAwait(false);

            promotion = await this._addPromotionItemPipeline.Run(
                            new PromotionItemArgument(
                                promotion,
                                "Habitat_Master|7042071|"),
                            context).ConfigureAwait(false);

            await this._addBenefitPipeline.Run(
                new PromotionActionModelArgument(
                    promotion,
                    new ActionModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        LibraryId = CartsConstants.CartSubtotalPercentOffAction,
                        Name = CartsConstants.CartSubtotalPercentOffAction,
                        Properties = new List<PropertyModel>
                                             {
                                                 new PropertyModel { Name = "PercentOff", Value = "50", IsOperator = false, DisplayType = "System.Decimal" }
                                             }
                    }),
                context).ConfigureAwait(false);

            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates cart 15 percent off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateCart15PctOffCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Cart15PctOffCouponPromotion", DateTimeOffset.UtcNow.AddDays(-5), DateTimeOffset.UtcNow.AddYears(1), "15% Off Cart (Coupon)", "15% Off Cart (Coupon)")
                    {
                        DisplayName = "15% Off Cart (Coupon)",
                        Description = "15% off Cart with subtotal of $50 or more (Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalCondition,
                            Name = CartsConstants.CartSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "50", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalPercentOffAction,
                            Name = CartsConstants.CartSubtotalPercentOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "PercentOff", Value = "15", DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABRTRNC15P"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the cart10 PCT off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <param name="date">The date.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateCart10PctOffCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context, DateTimeOffset date)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Cart10PctOffCouponPromotion", date, date.AddYears(1), "10% Off Cart (Coupon)", "10% Off Cart (Coupon)")
                    {
                        DisplayName = "10% Off Cart (Coupon)",
                        Description = "10% off Cart with subtotal of $50 or more (Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalCondition,
                            Name = CartsConstants.CartSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "50", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalPercentOffAction,
                            Name = CartsConstants.CartSubtotalPercentOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "PercentOff", Value = "10", DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABRTRNC10P"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the cart10 off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <param name="date">The date.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateCart10OffCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context, DateTimeOffset date)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Cart10OffCouponPromotion", date, date.AddYears(1), "$10 Off Cart (Coupon)", "$10 Off Cart (Coupon)")
                    {
                        DisplayName = "$10 Off Cart (Coupon)",
                        Description = "$10 off Cart with subtotal of $50 or more (Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalCondition,
                            Name = CartsConstants.CartSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "50", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalAmountOffAction,
                            Name = CartsConstants.CartSubtotalAmountOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "AmountOff", Value = "10", DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABRTRNC10A"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the disabled promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateDisabledPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "DisabledPromotion", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1), "Disabled", "Disabled")
                    {
                        DisplayName = "Disabled",
                        Description = "Disabled"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalCondition,
                            Name = CartsConstants.CartSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "5", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartSubtotalPercentOffAction,
                            Name = CartsConstants.CartSubtotalPercentOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "PercentOff", Value = "100", DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion.SetPolicy(new DisabledPolicy());
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        private async Task AssociateCatalogToBook(string bookName, string catalogName, CommercePipelineExecutionContext context)
        {
            // To persist entities conventionally and to prevent any race conditions, create a separate CommercePipelineExecutionContext object and CommerceContext object.
            var pipelineExecutionContext = new CommercePipelineExecutionContext(new CommerceContext(context.CommerceContext.Logger, context.CommerceContext.TelemetryClient)
            {
                GlobalEnvironment = context.CommerceContext.GlobalEnvironment,
                Environment = context.CommerceContext.Environment,
                Headers = new HeaderDictionary(context.CommerceContext.Headers.ToDictionary(x => x.Key, y => y.Value)) // Clone current context headers by shallow copy.
            }.PipelineContextOptions, context.CommerceContext.Logger);

            // To persist entities conventionally, remove policy keys in the newly created CommerceContext object.
            pipelineExecutionContext.CommerceContext.RemoveHeader(CoreConstants.PolicyKeys);

            var arg = new CatalogAndBookArgument(bookName, catalogName);
            await _associateCatalogToBookPipeline.Run(arg, pipelineExecutionContext).ConfigureAwait(false);
        }

        #endregion

        #region Line Promotions

        /// <summary>
        /// Creates line Touch Screen promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        private async Task CreateLineTouchScreenPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "LineHabitat34withTouchScreenPromotion", DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1), "Habitat Touch Screen 50% Off", "Habitat Touch Screen 50% Off")
                    {
                        DisplayName = "Habitat Touch Screen 50% Off",
                        Description = "50% off the Habitat 34.0 Cubic Refrigerator with Touchscreen item"
                    },
                    context).ConfigureAwait(false);

            promotion = await this._addPromotionItemPipeline.Run(
                            new PromotionItemArgument(
                                promotion,
                                "Habitat_Master|6042588|"),
                            context).ConfigureAwait(false);

            await this._addBenefitPipeline.Run(
                new PromotionActionModelArgument(
                    promotion,
                    new ActionModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        LibraryId = CartsConstants.CartItemSubtotalPercentOffAction,
                        Name = CartsConstants.CartItemSubtotalPercentOffAction,
                        Properties = new List<PropertyModel>
                                             {
                                                 new PropertyModel { Name = "PercentOff", Value = "50", IsOperator = false, DisplayType = "System.Decimal" },
                                                 new PropertyModel { Name = "TargetItemId", Value = "Habitat_Master|6042588|", IsOperator = false, DisplayType = "System.String" }
                                             }
                    }),
                context).ConfigureAwait(false);

            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the line laptop price promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        private async Task CreateLineLaptopPricePromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "LineHabitatLaptopPricePromotion", DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1), "Pay only $5", "Pay only $5")
                    {
                        DisplayName = "Pay only $5",
                        Description = "Pay only $5"
                    },
                    context).ConfigureAwait(false);

            promotion = await this._addPromotionItemPipeline.Run(
                            new PromotionItemArgument(
                                promotion,
                                "Habitat_Master|6042178|"),
                            context).ConfigureAwait(false);

            await this._addBenefitPipeline.Run(
                new PromotionActionModelArgument(
                    promotion,
                    new ActionModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        LibraryId = CartsConstants.CartItemSellPriceAction,
                        Name = CartsConstants.CartItemSellPriceAction,
                        Properties = new List<PropertyModel>
                                             {
                                                 new PropertyModel { Name = "SellPrice", Value = "5", IsOperator = false, DisplayType = "System.Decimal" },
                                                 new PropertyModel { Name = "TargetItemId", Value = "Habitat_Master|6042178|", IsOperator = false, DisplayType = "System.String" }
                                             }
                    }),
                context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABSELLPRICE"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the line TOuch Screen 5 off promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateLineTouchScreen5OffPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "LineHabitat34withTouchScreen5OffPromotion", DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddYears(1), "Habitat Touch Screen $5 Off Item", "Habitat Touch Screen $5 Off Item")
                    {
                        DisplayName = "Habitat Touch Screen $5 Off",
                        Description = "$5 off the Habitat 34.0 Cubic Refrigerator with Touchscreen item"
                    },
                    context).ConfigureAwait(false);

            promotion = await this._addPromotionItemPipeline.Run(
                            new PromotionItemArgument(
                                promotion,
                                "Habitat_Master|6042588|"),
                            context).ConfigureAwait(false);

            await this._addBenefitPipeline.Run(
                new PromotionActionModelArgument(
                    promotion,
                    new ActionModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        LibraryId = CartsConstants.CartItemSubtotalAmountOffAction,
                        Name = CartsConstants.CartItemSubtotalAmountOffAction,
                        Properties = new List<PropertyModel>
                                             {
                                                 new PropertyModel { Name = "AmountOff", Value = "5", IsOperator = false, DisplayType = "System.Decimal" },
                                                 new PropertyModel { Name = "TargetItemId", Value = "Habitat_Master|6042588|", IsOperator = false, DisplayType = "System.String" }
                                             }
                    }),
                context).ConfigureAwait(false);

            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the line mire laptop exclusive promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateLineExclusiveMiraLaptopPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "LineMiraLaptopExclusivePromotion", DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddYears(1), "Mira Laptop 50% Off Item (Exclusive)", "Mira Laptop 50% Off Item (Exclusive)")
                    {
                        DisplayName = "Mira Laptop 50% Off Item (Exclusive)",
                        Description = "50% off the Mira Laptop item (Exclusive)",
                        IsExclusive = true
                    },
                    context).ConfigureAwait(false);

            promotion = await this._addPromotionItemPipeline.Run(
                            new PromotionItemArgument(
                                promotion,
                                "Habitat_Master|6042179|"),
                            context).ConfigureAwait(false);

            await this._addBenefitPipeline.Run(
                new PromotionActionModelArgument(
                    promotion,
                    new ActionModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        LibraryId = CartsConstants.CartItemSubtotalPercentOffAction,
                        Name = CartsConstants.CartItemSubtotalPercentOffAction,
                        Properties = new List<PropertyModel>
                                             {
                                                 new PropertyModel { Name = "PercentOff", Value = "50", IsOperator = false, DisplayType = "System.Decimal" },
                                                 new PropertyModel { Name = "TargetItemId", Value = "Habitat_Master|6042179|", IsOperator = false, DisplayType = "System.String" }
                                             }
                    }),
                context).ConfigureAwait(false);

            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates line exclusive 20 percent off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateLineExclusive20PctOffCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Line20PctOffExclusiveCouponPromotion", DateTimeOffset.UtcNow.AddDays(-3), DateTimeOffset.UtcNow.AddYears(1), "20% Off Item (Exclusive Coupon)", "20% Off Item (Exclusive Coupon)")
                    {
                        IsExclusive = true,
                        DisplayName = "20% Off Item (Exclusive Coupon)",
                        Description = "20% off any item with subtotal of $50 or more (Exclusive Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalCondition,
                            Name = CartsConstants.CartAnyItemSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "25", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalPercentOffAction,
                            Name = CartsConstants.CartAnyItemSubtotalPercentOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "PercentOff", Value = "20", DisplayType = "System.Decimal" },
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "25", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABRTRNEL20P"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the line exclusive $20 off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateLineExclusive20OffCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Line20OffExclusiveCouponPromotion", DateTimeOffset.UtcNow.AddDays(-4), DateTimeOffset.UtcNow.AddYears(1), "$20 Off Item (Exclusive Coupon)", "$20 Off Item (Exclusive Coupon)")
                    {
                        IsExclusive = true,
                        DisplayName = "$20 Off Item (Exclusive Coupon)",
                        Description = "$20 off any item with subtotal of $50 or more (Exclusive Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalCondition,
                            Name = CartsConstants.CartAnyItemSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "25", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalAmountOffAction,
                            Name = CartsConstants.CartAnyItemSubtotalAmountOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "AmountOff", Value = "20", DisplayType = "System.Decimal" },
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "25", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABRTRNEL20A"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates line 5 percent off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateLine5PctOffCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Line5PctOffCouponPromotion", DateTimeOffset.UtcNow.AddDays(-5), DateTimeOffset.UtcNow.AddYears(1), "5% Off Item (Coupon)", "5% Off Item (Coupon)")
                    {
                        DisplayName = "5% Off Item (Coupon)",
                        Description = "5% off any item with subtotal of 10$ or more (Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalCondition,
                            Name = CartsConstants.CartAnyItemSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "10", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalPercentOffAction,
                            Name = CartsConstants.CartAnyItemSubtotalPercentOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "PercentOff", Value = "5", DisplayType = "System.Decimal" },
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "10", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABRTRNL5P"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates line 5 amount off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateLine5OffCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Line5OffCouponPromotion", DateTimeOffset.UtcNow.AddDays(-6), DateTimeOffset.UtcNow.AddYears(1), "$5 Off Item (Coupon)", "$5 Off Item (Coupon)")
                    {
                        DisplayName = "$5 Off Item (Coupon)",
                        Description = "$5 off any item with subtotal of $10 or more (Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalCondition,
                            Name = CartsConstants.CartAnyItemSubtotalCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "10", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartAnyItemSubtotalAmountOffAction,
                            Name = CartsConstants.CartAnyItemSubtotalAmountOffAction,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { Name = "AmountOff", Value = "5", DisplayType = "System.Decimal" },
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Subtotal", Value = "10", IsOperator = false, DisplayType = "System.Decimal" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "HABRTRNL5A"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates line 5 amount off coupon promotion.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task CreateFreeShippingFoodieCouponPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "FreeShippingForFoodieFirstOrder", DateTimeOffset.UtcNow.AddDays(-6), DateTimeOffset.UtcNow.AddYears(1),
                    "Free shipping for joining the Foodie email list (Coupon)", "Free shipping for joining the Foodie email list (Coupon)")
                    {
                        DisplayName = "Free shipping for joining the Foodie email list (Coupon)",
                        Description = "Free shipping for joining the Foodie email list (Coupon)"
                    },
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CustomersConstants.CurrentCustomerOrdersCountCondition,
                            Name = CustomersConstants.CurrentCustomerOrdersCountCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.IntegerEqualsOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[int],[int]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Count", Value = "1", IsOperator = false, DisplayType = "int" }
                                                 }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addBenefitPipeline.Run(
                    new PromotionActionModelArgument(
                        promotion,
                        new ActionModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = FulfillmentConstants.CartFreeShippingAction,
                            Name = FulfillmentConstants.CartFreeShippingAction
                        }),
                    context).ConfigureAwait(false);

            promotion = await this._addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "FREESHIPFOODIE"), context).ConfigureAwait(false);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        #endregion

        #region Bundle Promotions
        private async Task CreateBundleFitnessPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var promotion =
                await this._addPromotionPipeline.Run(
                    new AddPromotionArgument(book, "Free100GiftCardWithGymAndPlatinumTrainingPromotion",
                    DateTimeOffset.UtcNow.AddDays(-6),
                    DateTimeOffset.UtcNow.AddYears(1),
                    "Free $100 Gift Card With Habitat Striva Stationary Bike with Wifi and Sydney Training Platinum",
                    "Free $100 Gift Card With Habitat Striva Stationary Bike with Wifi and Sydney Training Platinum")
                    {
                        DisplayName = "Free $100 Gift Card With Habitat Striva Stationary Bike with Wifi and Sydney Training Platinum",
                        Description = "Free $100 Gift Card With Habitat Striva Stationary Bike with Wifi and Sydney Training Platinum"
                    },
                    context).ConfigureAwait(false);

            //Qualifications
            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartItemQuantityCondition,
                            Name = CartsConstants.CartItemQuantityCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalEqualityOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Quantity", Value = "1", IsOperator = false, DisplayType = "System.String" },
                                                     new PropertyModel { Name = "TargetItemId", Value = "Habitat_Master|8042103|", IsOperator = false, DisplayType = "System.String" }
                                               }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartItemQuantityCondition,
                            Name = CartsConstants.CartItemQuantityCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalEqualityOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Quantity", Value = "1", IsOperator = false, DisplayType = "System.String" },
                                                     new PropertyModel { Name = "TargetItemId", Value = "Habitat_Master|8042104|", IsOperator = false, DisplayType = "System.String" }
                                               }
                        }),
                    context).ConfigureAwait(false);

            promotion =
                await this._addQualificationPipeline.Run(
                    new PromotionConditionModelArgument(
                        promotion,
                        new ConditionModel
                        {
                            ConditionOperator = "And",
                            Id = Guid.NewGuid().ToString(),
                            LibraryId = CartsConstants.CartItemQuantityCondition,
                            Name = CartsConstants.CartItemQuantityCondition,
                            Properties = new List<PropertyModel>
                                                 {
                                                     new PropertyModel { IsOperator = true, Name = "Operator", Value = "Sitecore.Framework.Rules.DecimalEqualityOperator", DisplayType = "Sitecore.Framework.Rules.IBinaryOperator`2[[System.Decimal],[System.Decimal]], Sitecore.Framework.Rules.Abstractions" },
                                                     new PropertyModel { Name = "Quantity", Value = "1", IsOperator = false, DisplayType = "System.String" },
                                                     new PropertyModel { Name = "TargetItemId", Value = "Habitat_Master|6042986|56042988", IsOperator = false, DisplayType = "System.String" }
                                               }
                        }),
                    context).ConfigureAwait(false);

            //Benefits             
            await this._addBenefitPipeline.Run(
               new PromotionActionModelArgument(
                   promotion,
                   new ActionModel
                   {
                       Id = Guid.NewGuid().ToString(),
                       LibraryId = CartsConstants.CartItemSubtotalAmountOffAction,
                       Name = CartsConstants.CartItemSubtotalAmountOffAction,
                       Properties = new List<PropertyModel>
                                            {
                                                 new PropertyModel { Name = "AmountOff", Value = "100", IsOperator = false, DisplayType = "System.Decimal" },
                                                 new PropertyModel { Name = "TargetItemId", Value = "Habitat_Master|6042986|56042988", IsOperator = false, DisplayType = "System.String" }
                                            }
                   }),
               context).ConfigureAwait(false);


            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(promotion), context).ConfigureAwait(false);
        }

        #endregion
    }
}
