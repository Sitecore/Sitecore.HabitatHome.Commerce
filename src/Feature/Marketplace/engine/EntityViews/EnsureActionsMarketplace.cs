// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureActionsMarketplace.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.BusinessUsers;
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
    [PipelineDisplayName("EnsureActionsMarketplace")]
    public class EnsureActionsMarketplace : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureActionsMarketplace"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureActionsMarketplace(CommerceCommander commerceCommander)
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

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

            if (entityView.Name == "MarketplacesDashboard")
            {
                var businessUser = await this._commerceCommander.Command<BusinessUserCommander>().CurrentBusinessUser(context.CommerceContext);
                var ebayView = entityView.ChildViews.FirstOrDefault(p => p.Name == "EbayMarketplace");
                if (ebayView != null)
                {
                    //var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

                    if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                    {

                        var ebayBusinessUserComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                        if (!string.IsNullOrEmpty(ebayBusinessUserComponent.EbayToken))
                        {
                            ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-Configure",
                                DisplayName = $"Configure Ebay",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormConfigure",
                                UiHint = "",
                                Icon = "control_panel"
                            });

                            //ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            //{
                            //    Name = "Ebay-CreateInventorySet",
                            //    DisplayName = $"Create Inventory Set",
                            //    Description = "",
                            //    IsEnabled = true,
                            //    RequiresConfirmation = false,
                            //    EntityView = "Ebay-FormCreateInventorySet",
                            //    UiHint = "",
                            //    Icon = pluginPolicy.Icon
                            //});

                            ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-RemoveToken",
                                DisplayName = $"Remove Ebay User Token",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = true,
                                EntityView = "",
                                UiHint = "",
                                Icon = "barrier_open"
                            });

                            var listedItemsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "ListedItems");
                            if (listedItemsView != null)
                            {
                                listedItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
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

                                listedItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                {
                                    Name = "Ebay-EndListingAllItems",
                                    DisplayName = $"End All Item Listings on Ebay",
                                    Description = "",
                                    IsEnabled = true,
                                    RequiresConfirmation = true,
                                    EntityView = "",
                                    UiHint = "",
                                    Icon = "shelf_empty"
                                });

                            }

                            var pendingItemsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "PendingItems");
                            if (pendingItemsView != null)
                            {
                                pendingItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                {
                                    Name = "Ebay-PublishPending",
                                    DisplayName = $"Publish Pending Listings to Ebay",
                                    Description = "",
                                    IsEnabled = true,
                                    RequiresConfirmation = true,
                                    EntityView = "",
                                    UiHint = "",
                                    Icon = "shelf_full"
                                });
                            }

                        }
                        else
                        {
                            ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-RegisterToken",
                                DisplayName = $"Register an Ebay User Token",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormRegisterToken",
                                UiHint = "",
                                Icon = "barrier_closed"
                            });
                        }
                    }
                    else
                    {
                        ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                        {
                            Name = "Ebay-RegisterToken",
                            DisplayName = $"Register an Ebay User Token",
                            Description = "",
                            IsEnabled = true,
                            RequiresConfirmation = false,
                            EntityView = "Ebay-FormRegisterToken",
                            UiHint = "",
                            Icon = "barrier_closed"
                        });
                    }
                }
                return entityView;
            }

            return entityView;

        }
    }
}
