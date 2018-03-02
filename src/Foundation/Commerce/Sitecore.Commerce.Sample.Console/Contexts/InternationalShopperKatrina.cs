namespace Sitecore.Commerce.Sample.Contexts
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Payments;

    public class InternationalShopperKatrina
    {
        public InternationalShopperKatrina()
        {
            this.Context = new ShopperContext
            {
                Shop = Sample.Console.Program.DefaultStorefront,
                ShopperId = "InternationalShopperKatrinaId",
                Environment = "AdventureWorksShops",
                Language = "en-CA",
                Currency = "EUR",
                PolicyKeys = "ZeroMinionDelay|xActivityPerf",
                EffectiveDate = DateTimeOffset.Now,
                Components = new List<Component>()
                {
                    new PhysicalFulfillmentComponent
                    {
                        ShippingParty = new Party
                        {
                            FirstName = "Katrina",
                            LastName = "Fromage",
                            AddressName = "FulfillmentPartyName",
                            Address1 = "22 Rue Laval",
                            City = "Gatineau",
                            State = "QC",
                            Country = "CAD",
                            ZipPostalCode = "ABC123"
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
                            Address1 = "22 Rue Laval",
                            City = "Gatineau",
                            State = "QC",
                            Country = "CA",
                            ZipPostalCode = "ABC123"
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
                        EmailAddress = "katrina@domain.com",
                        EmailContent = "this is the content of the email"
                    }
                }
            };
        }

        public ShopperContext Context { get; set; }
    }
}
