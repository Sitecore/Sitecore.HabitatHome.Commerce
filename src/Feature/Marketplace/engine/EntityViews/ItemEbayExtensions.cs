using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.EBay.Engine.Commands;
using Sitecore.HabitatHome.Feature.EBay.Engine.Components;
using Sitecore.HabitatHome.Feature.EBay.Engine.Entities;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.EntityViews
{
    /// <summary>
    /// Defines a block which populates an EntityView for a Sample Page in the Sitecore Commerce Focused Commerce Experience.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Sitecore.HabitatHome.Feature.Ebay.Engine")]
    public class ItemEbayExtensions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEbayExtensions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ItemEbayExtensions(CommerceCommander commerceCommander)
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

            try
            {
                var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", new int?(), true);

                if (ebayConfig.HasComponent<EbayBusinessUserComponent>()) {
                    var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();

                    if (entityView.EntityId.Contains("Entity-Category-"))
                    {


                        var sellableItemsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "SellableItems") as EntityView;
                        if (sellableItemsView != null)
                        {
                            foreach (var sellableItemViewItem in sellableItemsView.ChildViews)
                            {
                                var sellableItemViewItemAsEntityView = sellableItemViewItem as EntityView;

                                var sellableItemId = (sellableItemViewItemAsEntityView).ItemId;
                                var foundEntity = context.CommerceContext.GetObjects<CommerceEntity>().FirstOrDefault(p => p.Id == sellableItemId);
                                if (foundEntity != null)
                                {
                                    var sellableItem = foundEntity as SellableItem;
                                    if (sellableItem.HasComponent<EbayItemComponent>())
                                    {
                                        var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                                        if (ebayItemComponent.Status != "Ended")
                                        {
                                            sellableItemViewItemAsEntityView.Properties.Insert(0,
                                            new ViewProperty
                                            {
                                                Name = "Marketplaces",
                                                IsHidden = false,
                                                IsReadOnly = true,
                                                OriginalType = "Html",
                                                UiType = "Html",
                                                RawValue = "<img alt='This is the alternate' height=20 width=40 src='https://www.paypalobjects.com/webstatic/en_AR/mktg/merchant/pages/sell-on-ebay/image-ebay.png' style=''/>"
                                            });
                                        }
                                        else
                                        {
                                            sellableItemViewItemAsEntityView.Properties.Insert(0,
                                            new ViewProperty
                                            {
                                                Name = "",
                                                IsHidden = false,
                                                IsReadOnly = true,
                                                UiType = "",
                                                RawValue = ""
                                            });
                                        }
                                    }
                                    else
                                    {
                                        sellableItemViewItemAsEntityView.Properties.Insert(0,
                                            new ViewProperty
                                            {
                                                Name = "",
                                                IsHidden = false,
                                                IsReadOnly = true,
                                                UiType = "",
                                                RawValue = ""
                                            });
                                    }
                                }
                            }
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

                    if (!(entityViewArgument.Entity is SellableItem))
                    {
                        return entityView;
                    }

                    var item = entityViewArgument.Entity as SellableItem;
                    if (item.HasComponent<EbayItemComponent>())
                    {
                        var ebayItemComponent = item.GetComponent<EbayItemComponent>();

                        var childView = new EntityView
                        {
                            Name = "Ebay Marketplace Item",
                            UiHint = "Flat",
                            Icon = "market_stand",
                            DisplayRank = 200,
                            EntityId = item.Id,
                            ItemId = ""
                        };
                        entityView.ChildViews.Add(childView);

                        childView.Properties.Add(
                        new ViewProperty
                        {
                            Name = "",
                            IsHidden = false,
                            IsReadOnly = true,
                            OriginalType = "Html",
                            UiType = "Html",
                            RawValue = "<img alt='This is the alternate' height=50 width=100 src='https://www.paypalobjects.com/webstatic/en_AR/mktg/merchant/pages/sell-on-ebay/image-ebay.png' style=''/>"
                        });

                        childView.Properties.Add(
                            new ViewProperty
                            {
                                Name = "EbayId",
                                IsHidden = false,
                                IsReadOnly = true,
                                UiType = "Html",
                                OriginalType = "Html",
                                RawValue = $"<a href='http://cgi.sandbox.ebay.com/ws/eBayISAPI.dll?ViewItem&item={ebayItemComponent.EbayId}&ssPageName=STRK:MESELX:IT&_trksid=p3984.m1558.l2649#ht_500wt_1157' target='_blank'>{ebayItemComponent.EbayId}</a> "
                            });

                        childView.Properties.Add(
                            new ViewProperty
                            {
                                Name = "Status",
                                IsHidden = false,
                                IsReadOnly = true,
                                RawValue = ebayItemComponent.Status
                            });

                        if (ebayItemComponent.Status == "Ended")
                        {
                            childView.Properties.Add(
                            new ViewProperty
                            {
                                Name = "ReasonEnded",
                                DisplayName = "Reason Ended",
                                IsHidden = false,
                                IsReadOnly = true,
                                RawValue = ebayItemComponent.ReasonEnded
                            });
                        }

                        //var history = "--------------------<BR>";
                        //var history = "AAAAAAAAAAAAAAAAAAAA<BR>";
                        var history = "========================================<BR>";
                        foreach (var historyItem in ebayItemComponent.History)
                        {
                            history = history + $"{historyItem.EventDate.ToString("yyyy-MMM-dd hh:mm")}-{historyItem.EventMessage}-{historyItem.EventUser.Replace(@"sitecore\", "")}<BR>";
                        }

                        childView.Properties.Add(
                            new ViewProperty
                            {
                                Name = "History",
                                IsHidden = false,
                                IsReadOnly = true,
                                UiType = "Html",
                                OriginalType = "Html",
                                RawValue = history
                            });

                        //var ebayTime = await this._commerceCommander.Command<EbayCommand>().GetEbayTime(context.CommerceContext);
                        if (ebayItemComponent.Status == "Listed")
                        {
                            if (!string.IsNullOrEmpty(ebayItemComponent.EbayId))
                            {
                                var ebayItem = await this._commerceCommander.Command<EbayCommand>().GetItem(context.CommerceContext, ebayItemComponent.EbayId).ConfigureAwait(false);
                                childView.Properties.Add(
                                new ViewProperty
                                {
                                    Name = "EndTime",
                                    IsHidden = false,
                                    IsReadOnly = true,
                                    RawValue = ebayItem.ListingDetails.EndTime
                                });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Sku",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.SKU
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Price",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = new Money("USD", System.Convert.ToDecimal(ebayItem.StartPrice.Value))
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Country",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.Country.ToString()
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Currency",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.Currency.ToString()
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Location",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.Location
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "ListingDuration",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.ListingDuration
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Quantity",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.Quantity
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "PrimaryCategory",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.PrimaryCategory.CategoryID
                                    });


                                var ebayItemFeesView = new EntityView
                                {
                                    Name = "Ebay Item Fees",
                                    UiHint = "Flat",
                                    Icon = "market_stand",
                                    DisplayRank = 200,
                                    EntityId = item.Id,
                                    ItemId = ""
                                };
                                entityView.ChildViews.Add(ebayItemFeesView);

                                foreach (var fee in ebayItemComponent.Fees)
                                {
                                    ebayItemFeesView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = fee.Name,
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = fee.Adjustment
                                    });
                                }


                                //childView.Properties.Add(
                                //    new ViewProperty
                                //    {
                                //        Name = "Returns",
                                //        IsHidden = false,
                                //        IsReadOnly = true,
                                //        RawValue = ebayItem.ReturnPolicy.ReturnsAcceptedOption
                                //    });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.DoActionEndItem.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "ItemEbayExtensions.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
            }
            return entityView;
            
        }
    }
}
