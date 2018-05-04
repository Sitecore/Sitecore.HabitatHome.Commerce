using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.XA.Feature.Cart.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.ExtensionMethods;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Script.Serialization;

namespace Sitecore.Feature.ShoppingCartLines.Models.JsonResults
{
    public class ShoppingCartJsonResult : BaseJsonResult
    {
        public ShoppingCartJsonResult(IStorefrontContext storefrontContext, IModelProvider modelProvider)
          : base(storefrontContext)
        {
            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            this.ModelProvider = modelProvider;
        }

        [ScriptIgnore]
        public IModelProvider ModelProvider { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<Sitecore.Feature.ShoppingCartLines.Models.JsonResults.ShoppingCartLineJsonResult> Lines { get; set; }

        public string Email { get; set; }

        public string Subtotal { get; set; }

        public string TaxTotal { get; set; }

        public string Total { get; set; }

        public string ShippingTotal { get; set; }

        public Decimal TotalAmount { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<ShippingInfoJsonResult> Shipments { get; protected set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<PaymentInfoJsonResult> Payments { get; protected set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<AddressJsonResult> Parties { get; set; }

        public PartyLinkJsonResult AccountingParty { get; set; }

        public string Discount { get; set; }

        public IEnumerable<string> PromoCodes { get; set; }

        public virtual void Initialize(Sitecore.Commerce.Entities.Carts.Cart cart, dynamic cartLineList)
        {
            List<dynamic> expandedCartLines = (List<dynamic>)cartLineList;
            this.Email = cart.Email;
            this.TaxTotal = cart.Total.TaxTotal.Amount.ToCurrency();
            this.Total = cart.Total.Amount.ToCurrency();
            this.TotalAmount = cart.Total.Amount;
            this.PromoCodes = cart.GetPropertyValue("PromoCodes") as IEnumerable<string>;
            this.Lines = new List<Sitecore.Feature.ShoppingCartLines.Models.JsonResults.ShoppingCartLineJsonResult>();
            foreach (CartLine line in cart.Lines)
            {
                Sitecore.Feature.ShoppingCartLines.Models.JsonResults.ShoppingCartLineJsonResult model = this.ModelProvider.GetModel<Sitecore.Feature.ShoppingCartLines.Models.JsonResults.ShoppingCartLineJsonResult>();
                
                model.Initialize(line);

                dynamic expandedCartLine = expandedCartLines.Where(l => l.ExternalCartLineId == line.ExternalCartLineId).FirstOrDefault();
                if (expandedCartLine != null)
                    model.SetProductType(expandedCartLine);
                this.Lines.Add(model);
            }
            this.Shipments = new List<ShippingInfoJsonResult>();
            if (cart.Shipping != null && cart.Shipping.Any<ShippingInfo>())
            {
                foreach (ShippingInfo shippingInfo in cart.Shipping)
                {
                    ShippingInfoJsonResult model = this.ModelProvider.GetModel<ShippingInfoJsonResult>();
                    model.Initialize(shippingInfo);
                    this.Shipments.Add(model);
                }
            }
            this.Parties = new List<AddressJsonResult>();
            if (cart.Parties != null && cart.Parties.Any<Party>())
            {
                foreach (Party party in cart.Parties)
                {
                    AddressJsonResult model = this.ModelProvider.GetModel<AddressJsonResult>();
                    model.Initialize(party);
                    this.Parties.Add(model);
                }
            }
            this.Payments = new List<PaymentInfoJsonResult>();
            if (cart.Payment != null && cart.Payment.Any<PaymentInfo>())
            {
                foreach (PaymentInfo paymentInfo in cart.Payment)
                {
                    PaymentInfoJsonResult model = this.ModelProvider.GetModel<PaymentInfoJsonResult>();
                    model.Initialize(paymentInfo);
                    this.Payments.Add(model);
                }
            }
            if (cart.AccountingCustomerParty == null)
                return;
            this.AccountingParty = this.ModelProvider.GetModel<PartyLinkJsonResult>();
            this.AccountingParty.Name = cart.AccountingCustomerParty.Name;
            this.AccountingParty.PartyID = cart.AccountingCustomerParty.PartyID;

            if (!(cart is CommerceCart))
                return;
            CommerceTotal total = (CommerceTotal)cart.Total;
            this.Discount = (cart.Lines.Sum<CartLine>((Func<CartLine, Decimal>)(lineitem => ((CommerceTotal)lineitem.Total).LineItemDiscountAmount)) + ((CommerceTotal)cart.Total).OrderLevelDiscountAmount).ToCurrency();
            this.Subtotal = total.Subtotal.ToCurrency();
            this.ShippingTotal = total.ShippingTotal.ToCurrency();
        }
    }
}