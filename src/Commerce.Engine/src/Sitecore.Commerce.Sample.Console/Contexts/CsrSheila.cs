namespace Sitecore.Commerce.Sample.Contexts
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Payments;

    public class CsrSheila
    {
        public CsrSheila()
        {
            this.Context = new ShopperContext
            {
                Shop = Sample.Console.Program.DefaultStorefront,
                ShopperId = "DevOpsId",
                Environment = "AdventureWorksShops",
                Language = "en-US",
                Currency = "USD",
                PolicyKeys = "ZeroMinionDelay|xActivityPerf",
                EffectiveDate = DateTimeOffset.Now,
                Components = new List<Component>
                {
                    new PhysicalFulfillmentComponent
                    {
                        ShippingParty = new Party
                        {
                            FirstName = "Jeff",
                            LastName = "Key",
                            AddressName = "FulfillmentPartyName",
                            Address1 = "326 Duval Street",
                            City = "Key West",
                            State = "FL",
                            Country = "US",
                            ZipPostalCode = "33040"
                        },
                        FulfillmentMethod = new EntityReference
                        {
                            EntityTarget = "B146622D-DC86-48A3-B72A-05EE8FFD187A",
                            Name = "Ground"
                        }
                    },                   
                    new FederatedPaymentComponent
                    {
                        PaymentMethodNonce = "fake-valid-nonce",
                        BillingParty = new Party
                        {
                            FirstName = "Jeff",
                            LastName = "Key",
                            AddressName = "PaymentPartyName",
                            Address1 = "326 Duval Street",
                            City = "Key West",
                            StateCode = "WA",
                            CountryCode = "US",
                            ZipPostalCode = "33040"
                        },
                        PaymentMethod = new EntityReference { EntityTarget = "0CFFAB11-2674-4A18-AB04-228B1F8A1DEC", Name = "Federated" }
                    },
                    new ElectronicFulfillmentComponent
                    {
                        FulfillmentMethod = new EntityReference
                        {
                            EntityTarget = "8A23234F-8163-4609-BD32-32D9DD6E32F5",
                            Name = "Email"
                        },
                        EmailAddress = "sheila@domain.com",
                        EmailContent = "this is the content of the email"
                    }
                }
            };
        }

        public ShopperContext Context { get; set; }
    }
}
