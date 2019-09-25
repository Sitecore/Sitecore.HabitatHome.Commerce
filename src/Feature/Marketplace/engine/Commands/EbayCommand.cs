// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbayCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eBay.Service.Call;
using eBay.Service.Core.Sdk;
using eBay.Service.Core.Soap;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.HabitatHome.Feature.EBay.Engine.Components;
using Sitecore.HabitatHome.Feature.EBay.Engine.Entities;
using Sitecore.HabitatHome.Feature.EBay.Engine.Models;
using Sitecore.HabitatHome.Feature.EBay.Engine.Pipelines;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Defines the SampleCommand command.
    /// </summary>
    public class EbayCommand : CommerceCommand
    {
        private readonly IPrepareEbayItemPipeline _pipeline;
        private readonly CommerceCommander _commerceCommander;
        private static ApiContext apiContext; 

        public EbayCommand(IPrepareEbayItemPipeline pipeline, CommerceCommander commerceCommander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._pipeline = pipeline;
            this._commerceCommander = commerceCommander;
        }
        
        public async Task<bool> PublishPending(CommerceContext commerceContext)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                try
                {
                    var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(commerceContext);

                    var ebayPendingSellableItems = await this._commerceCommander.Command<ListCommander>()
                                .GetListItems<SellableItem>(commerceContext, "Ebay_Pending", 0, 10).ConfigureAwait(false);

                    foreach (var sellableItem in ebayPendingSellableItems)
                    {
                        var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                     
                        var ebayItem = await this._commerceCommander.Command<EbayCommand>().AddItem(commerceContext, sellableItem);
                        ebayItem.ListingDuration = "Days_10";
                        ebayItemComponent.EbayId = ebayItem.ItemID;
                        ebayItemComponent.Status = "Listed";
                        sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");

                        var persistResult = await this._commerceCommander.PersistEntity(commerceContext, sellableItem).ConfigureAwait(false);

                        var listRemoveResult = await this._commerceCommander.Command<ListCommander>()
                            .RemoveItemsFromList(commerceContext, "Ebay_Pending", new List<String>() { sellableItem.Id }).ConfigureAwait(false);

                    }
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"Ebay.PublishPendingCommand.Exception: Message={ex.Message}");
                }
                
                return true;
            }
        }

        public async Task<ItemType> AddItem(CommerceContext commerceContext, SellableItem sellableItem)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                try
                {
                    //Instantiate the call wrapper class
                    var apiCall = new AddFixedPriceItemCall(await GetEbayContext(commerceContext).ConfigureAwait(false));

                    var item = await PrepareItem(commerceContext, sellableItem).ConfigureAwait(false);

                    //Send the call to eBay and get the results
                    FeeTypeCollection feeTypeCollection = apiCall.AddFixedPriceItem(item);

                    foreach (var feeItem in feeTypeCollection)
                    {
                        var fee = feeItem as FeeType;
                        ebayItemComponent.Fees.Add(new AwardedAdjustment { Adjustment = new Money(fee.Fee.currencyID.ToString(), System.Convert.ToDecimal(fee.Fee.Value)), AdjustmentType = "Fee", Name = fee.Name });
                    }

                    ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = "Listing Added", EventUser = commerceContext.CurrentCsrId() });
                    ebayItemComponent.EbayId = item.ItemID;
                    ebayItemComponent.Status = "Listed";
                    sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");
                    await commerceContext.AddMessage("Info", "EbayCommand.AddItem", new []{ item.ItemID }, $"Item Listed:{item.ItemID}").ConfigureAwait(false);

                    return item;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("It looks like this listing is for an item you already have on eBay"))
                    {
                        var existingId = ex.Message.Substring(ex.Message.IndexOf("(") + 1);
                        existingId = existingId.Substring(0, existingId.IndexOf(")"));
                        await commerceContext.AddMessage("Warn", "EbayCommand.AddItem", new [] { existingId }, $"ExistingId:{existingId}-ComponentId:{ebayItemComponent.EbayId}").ConfigureAwait(false);

                        ebayItemComponent.EbayId = existingId;
                        ebayItemComponent.Status = "Listed";
                        ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = "Existing Listing Linked", EventUser = commerceContext.CurrentCsrId() });
                        sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");
                    }
                    else
                    {
                        commerceContext.Logger.LogError($"Ebay.AddItem.Exception: Message={ex.Message}");
                        await commerceContext.AddMessage("Error", "Ebay.AddItem.Exception", new [] { ex }, ex.Message).ConfigureAwait(false);

                        ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = $"Error-{ex.Message}", EventUser = commerceContext.CurrentCsrId() });
                    }
                }
                return new ItemType();
            }
        }

        public async Task<ItemType> PrepareItem(CommerceContext commerceContext, SellableItem sellableItem)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                //Instantiate the call wrapper class
                var apiCall = new AddFixedPriceItemCall(await GetEbayContext(commerceContext).ConfigureAwait(false));

                var item = await this._commerceCommander.Pipeline<IPrepareEbayItemPipeline>().Run(sellableItem, commerceContext.PipelineContextOptions).ConfigureAwait(false);
                
                item.Description = sellableItem.Description;
                item.Title = sellableItem.DisplayName;
                item.SubTitle = "Test Item";

                var listPricingPolicy = sellableItem.GetPolicy<ListPricingPolicy>();
                var listPrice = listPricingPolicy.Prices.FirstOrDefault();

                item.StartPrice = new AmountType { currencyID = CurrencyCodeType.USD, Value = System.Convert.ToDouble(listPrice.Amount, System.Globalization.CultureInfo.InvariantCulture) };

                item.ConditionID = 1000;  //new

                item.PaymentMethods = new BuyerPaymentMethodCodeTypeCollection();
                item.PaymentMethods.Add(BuyerPaymentMethodCodeType.PayPal);
                item.PaymentMethods.Add(BuyerPaymentMethodCodeType.VisaMC);
                item.PayPalEmailAddress = "test@test.com";
                item.PostalCode = "98014";

                item.DispatchTimeMax = 3;
                item.ShippingDetails = new ShippingDetailsType();
                item.ShippingDetails.ShippingServiceOptions = new ShippingServiceOptionsTypeCollection();

                item.ShippingDetails.ShippingType = ShippingTypeCodeType.Flat;

                ShippingServiceOptionsType shipservice1 = new ShippingServiceOptionsType();
                shipservice1.ShippingService = "USPSPriority";
                shipservice1.ShippingServicePriority = 1;
                shipservice1.ShippingServiceCost = new AmountType();
                shipservice1.ShippingServiceCost.currencyID = CurrencyCodeType.USD;
                shipservice1.ShippingServiceCost.Value = 5.0;

                shipservice1.ShippingServiceAdditionalCost = new AmountType();
                shipservice1.ShippingServiceAdditionalCost.currencyID = CurrencyCodeType.USD;
                shipservice1.ShippingServiceAdditionalCost.Value = 1.0;

                item.ShippingDetails.ShippingServiceOptions.Add(shipservice1);


                //ShippingServiceOptionsType shipservice2 = new ShippingServiceOptionsType();
                //shipservice2.ShippingService = "US_Regular";
                //shipservice2.ShippingServicePriority = 2;
                //shipservice2.ShippingServiceCost = new AmountType();
                //shipservice2.ShippingServiceCost.currencyID = CurrencyCodeType.USD;
                //shipservice2.ShippingServiceCost.Value = 1.0;

                //shipservice2.ShippingServiceAdditionalCost = new AmountType();
                //shipservice2.ShippingServiceAdditionalCost.currencyID = CurrencyCodeType.USD;
                //shipservice2.ShippingServiceAdditionalCost.Value = 1.0;

                //item.ShippingDetails.ShippingServiceOptions.Add(shipservice2);

                //item.Variations.

                item.ReturnPolicy = new ReturnPolicyType { ReturnsAcceptedOption = "ReturnsAccepted" };

                //Add pictures
                item.PictureDetails = new PictureDetailsType();

                //Specify GalleryType
                item.PictureDetails.GalleryType = GalleryTypeCodeType.None;
                item.PictureDetails.GalleryTypeSpecified = true;

                return item;
            }
        }

        public async Task<bool> EndItemListing(CommerceContext commerceContext, SellableItem sellableItem, string reason)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                //Instantiate the call wrapper class

                try
                {
                    var apiCall = new EndItemCall(await GetEbayContext(commerceContext));

                    if (sellableItem.HasComponent<EbayItemComponent>())
                    {
                        var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                        var reasonCodeType = EndReasonCodeType.NotAvailable;

                        switch (reason)
                        {
                            case "NotAvailable":
                                reasonCodeType = EndReasonCodeType.NotAvailable;
                                break;
                            case "CustomCode":
                                reasonCodeType = EndReasonCodeType.CustomCode;
                                break;
                            case "Incorrect":
                                reasonCodeType = EndReasonCodeType.Incorrect;
                                break;
                            case "LostOrBroken":
                                reasonCodeType = EndReasonCodeType.LostOrBroken;
                                break;
                            case "OtherListingError":
                                reasonCodeType = EndReasonCodeType.OtherListingError;
                                break;
                            case "SellToHighBidder":
                                reasonCodeType = EndReasonCodeType.SellToHighBidder;
                                break;
                            case "Sold":
                                reasonCodeType = EndReasonCodeType.Sold;
                                break;
                            default:
                                reasonCodeType = EndReasonCodeType.CustomCode;
                                break;
                        }

                        if (string.IsNullOrEmpty(ebayItemComponent.EbayId))
                        {
                            ebayItemComponent.Status = "LostSync";
                        }
                        else
                        {
                            if (ebayItemComponent.Status != "Ended")
                            {
                                //Call Ebay and End the Item Listing
                                try
                                {
                                    apiCall.EndItem(ebayItemComponent.EbayId, reasonCodeType);
                                    ebayItemComponent.Status = "Ended";
                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message == "The auction has already been closed.")
                                    {
                                        //Capture a case where the listing has expired naturally and it can now no longer be ended.
                                        reason = "Expired";
                                        ebayItemComponent.Status = "Ended";
                                    }
                                    else
                                    {
                                        commerceContext.Logger.LogError(ex, $"EbayCommand.EndItemListing.Exception: Message={ex.Message}");
                                        await commerceContext.AddMessage("Error", "EbayCommand.EndItemListing", new [] { ex }, ex.Message).ConfigureAwait(false);
                                    }
                                }
                            }
                        }

                        ebayItemComponent.ReasonEnded = reason;

                        ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = "Listing Ended", EventUser = commerceContext.CurrentCsrId() });

                        sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Ended");

                        var persistResult = await this._commerceCommander.PersistEntity(commerceContext, sellableItem).ConfigureAwait(false);

                        var listRemoveResult = await this._commerceCommander.Command<ListCommander>()
                            .RemoveItemsFromList(commerceContext, "Ebay_Listed", new List<String>() { sellableItem.Id }).ConfigureAwait(false);

                    }

                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"Ebay.EndItemListing.Exception: Message={ex.Message}");
                    await commerceContext.AddMessage("Error", "Ebay.EndItemListing.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
                }

                return true;
            }
        }

        public async Task<ItemType> RelistItem(CommerceContext commerceContext, SellableItem sellableItem)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                //Instantiate the call wrapper class
                var apiCall = new RelistItemCall(await GetEbayContext(commerceContext).ConfigureAwait(false));

                if (sellableItem.HasComponent<EbayItemComponent>())
                {
                    var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                    try
                    {
                        var item = await PrepareItem(commerceContext, sellableItem);
                        item.ItemID = ebayItemComponent.EbayId;

                        //item.ListingDuration = "Days_" + 10;

                        //Send the call to eBay and get the results
                        var feeResult = apiCall.RelistItem(item, new StringCollection());

                        ebayItemComponent.EbayId = item.ItemID;

                        ebayItemComponent.Status = "Listed";
                        sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");

                        foreach (var feeItem in feeResult)
                        {
                            var fee = feeItem as FeeType;
                            ebayItemComponent.Fees.Add(new AwardedAdjustment { Adjustment = new Money(fee.Fee.currencyID.ToString(), System.Convert.ToDecimal(fee.Fee.Value)), AdjustmentType = "Fee", Name = fee.Name });
                        }

                        ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = "Listing Relisted", EventUser = commerceContext.CurrentCsrId() });

                        return item;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("It looks like this listing is for an item you already have on eBay"))
                        {
                            var existingId = ex.Message.Substring(ex.Message.IndexOf("(") + 1);
                            existingId = existingId.Substring(0, existingId.IndexOf(")"));
                            await commerceContext.AddMessage("Warn", "Ebay.RelistItem", new Object[] { }, $"ExistingId:{existingId}-ComponentId:{ebayItemComponent.EbayId}").ConfigureAwait(false);
                            ebayItemComponent.EbayId = existingId;
                            ebayItemComponent.Status = "Listed";
                            sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");
                        }
                        else
                        {
                            commerceContext.Logger.LogError($"Ebay.RelistItem.Exception: Message={ex.Message}");
                            await commerceContext.AddMessage("Error", "Ebay.RelistItem.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    commerceContext.Logger.LogError($"EbayCommand.RelistItem.Exception: Message=ebayCommand.RelistItem.NoEbayItemComponent");
                    await commerceContext.AddMessage("Error", "Ebay.RelistItem.Exception", new Object[] { }, "ebayCommand.RelistItem.NoEbayItemComponent").ConfigureAwait(false);
                }

                return new ItemType();
            }
        }

        public async Task<ItemType> GetItem(CommerceContext commerceContext, string itemId)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                //Instantiate the call wrapper class
                var apiCall = new GetItemCall(await GetEbayContext(commerceContext).ConfigureAwait(false));

                try
                {
                    var result = apiCall.GetItem(itemId);
                    return result;
                }
                catch (Exception ex)
                {                    
                    await commerceContext.AddMessage("Warn", "Ebay.GetItem.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
                }
                return new ItemType();
            }
        }


        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<DateTime> GetEbayTime(CommerceContext commerceContext)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {                
                //Instantiate the call wrapper class
                GeteBayOfficialTimeCall apiCall = new GeteBayOfficialTimeCall(await GetEbayContext(commerceContext).ConfigureAwait(false));

                //Send the call to eBay and get the results
                try
                {
                    var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(commerceContext, "Entity-EbayConfigEntity-Global", new int?(), true).ConfigureAwait(false);

                    if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                    {
                        var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                        if (!string.IsNullOrEmpty(ebayConfigComponent.EbayToken))
                        {
                            DateTime officialTime = apiCall.GeteBayOfficialTime();
                            return officialTime;
                        }
                    }    
                }
                catch(Exception ex)
                {
                    commerceContext.Logger.LogError($"Ebay.GetEbayTime.Exception: Message={ex.Message}");
                    await commerceContext.AddMessage("Error", "Ebay.GetEbayTime.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
                }

                //Handle the result returned
                //Console.WriteLine("eBay official Time: " + officialTime);
                
                return new DateTime();
            }
        }

        public async Task<SuggestedCategoryTypeCollection> GetSuggestedCategories(CommerceContext commerceContext, string query)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                try
                {
                    var getSuggestedCategories = new GetSuggestedCategoriesCall(await GetEbayContext(commerceContext).ConfigureAwait(false));
                    var result = getSuggestedCategories.GetSuggestedCategories(query);
                    
                    return result;
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"Ebay.GetCategories.Exception: Message={ex.Message}");
                    //await commerceContext.AddMessage("Error", "Ebay.GetCategories.Exception", new Object[] { ex }, ex.Message);
                }
                return new SuggestedCategoryTypeCollection();
            }
        }

        /// <summary>
        /// Instantiating and setting ApiContext
        /// </summary>
        /// <param name="commerceContext"></param>
        /// <returns></returns>
        public async Task<ApiContext> GetEbayContext(CommerceContext commerceContext)
        {
            //apiContext is a singleton,
            if (apiContext != null)
            {
                return apiContext;
            }
            else
            {
                apiContext = new ApiContext();
                var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(commerceContext, "Entity-EbayConfigEntity-Global", new int?(), true).ConfigureAwait(false);

                if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                {
                    var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();

                    //Supply user token
                    ApiCredential apiCredential = new ApiCredential();
                    apiContext.ApiCredential = apiCredential;

                    if (!string.IsNullOrEmpty(ebayConfigComponent.EbayToken))
                    {
                        apiCredential.eBayToken = ebayConfigComponent.EbayToken;
                    }
                }

                //supply Api Server Url
                apiContext.SoapApiServerUrl = "https://api.sandbox.ebay.com/wsapi";

                //Specify site: here we use US site
                apiContext.Site = SiteCodeType.US;

                return apiContext;
            } 
        } 

    }
}