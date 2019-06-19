// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureActions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;            
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.EBay.Engine.Components;
using Sitecore.HabitatHome.Feature.EBay.Engine.Entities;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.EntityViews
{
    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureActions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureActions(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<Policies.PluginPolicy>();

            var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true).ConfigureAwait(false);

            if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
            {
                var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                if (!string.IsNullOrEmpty(ebayConfigComponent.EbayToken))
                {
                    if (entityView.EntityId.Contains("Entity-Category-"))
                    {
                        var sellableItemsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "SellableItems") as EntityView;
                        if (sellableItemsView != null)
                        {
                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-StartSelling",
                                DisplayName = $"Start Selling Item On Ebay",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormStartSelling",
                                UiHint = "",
                                Icon = "shelf_full"
                            });

                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-EndItem",
                                DisplayName = $"End Item Listing on Ebay",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormEndItem",
                                UiHint = "",
                                Icon = "shelf_empty"
                            });

                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-StartSellingAll",
                                DisplayName = $"List All Items on Ebay for this Category ",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormStartSellingAll",
                                UiHint = "",
                                Icon = "shelf_full"
                            });

                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-EndListingAllItems",
                                DisplayName = $"End All Listings on Ebay for this Category",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = true,
                                EntityView = "",
                                UiHint = "",
                                Icon = "shelf_empty"
                            });
                        }
                        return entityView;
                    }

                    if (entityView.Name != "Master")
                    {
                        return entityView;
                    }
                    if (!entityView.EntityId.Contains("Entity-SellableItem-"))
                    {
                        return entityView;
                    }

                    var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

                    if (entityViewArgument.Entity is SellableItem)
                    {
                        var sellableItem = entityViewArgument.Entity as SellableItem;
                        if (sellableItem.HasComponent<EbayItemComponent>())
                        {
                            var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                            var ebayView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Ebay Marketplace Item");
                            if (ebayView != null)
                            {

                                if (ebayItemComponent.Status == "Listed")
                                {
                                    ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                    {
                                        Name = "Ebay-EndItem",
                                        DisplayName = $"End Listing On Ebay",
                                        Description = "",
                                        IsEnabled = true,
                                        RequiresConfirmation = false,
                                        EntityView = "Ebay-FormEndItem",
                                        UiHint = "",
                                        Icon = "shelf_empty"
                                    });
                                }
                                else if (ebayItemComponent.Status == "Pending")
                                {
                                    ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                    {
                                        Name = "Ebay-EndItem",
                                        DisplayName = $"End Listing On Ebay",
                                        Description = "",
                                        IsEnabled = true,
                                        RequiresConfirmation = false,
                                        EntityView = "Ebay-FormEndItem",
                                        UiHint = "",
                                        Icon = "shelf_empty"
                                    });
                                }
                                else if (ebayItemComponent.Status == "LostSync")
                                {
                                    ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                    {
                                        Name = "Ebay-FixSyncItem",
                                        DisplayName = $"Fix Sync of Ebay Item",
                                        Description = "",
                                        IsEnabled = true,
                                        RequiresConfirmation = true,
                                        EntityView = "",
                                        UiHint = "",
                                        Icon = "wrench"
                                    });
                                }
                                else
                                {
                                    ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                    {
                                        Name = "Ebay-StartSelling",
                                        DisplayName = $"List this Item On Ebay",
                                        Description = "",
                                        IsEnabled = true,
                                        RequiresConfirmation = false,
                                        EntityView = "Ebay-FormStartSelling",
                                        UiHint = "",
                                        Icon = "shelf_full"
                                    });
                                    if (ebayItemComponent.Status == "Ended")
                                    {
                                        ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                        {
                                            Name = "Ebay-ForgetItem",
                                            DisplayName = $"Forget Ebay Component from this Item",
                                            Description = "",
                                            IsEnabled = true,
                                            RequiresConfirmation = true,
                                            EntityView = "",
                                            UiHint = "",
                                            Icon = "shelf_full"
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            entityView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-StartSelling",
                                DisplayName = $"List this Item On Ebay",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormStartSelling",
                                UiHint = "",
                                Icon = "shelf_full"
                            });
                        }
                    }
                }
            }

            return entityView;
        }
    }
}
