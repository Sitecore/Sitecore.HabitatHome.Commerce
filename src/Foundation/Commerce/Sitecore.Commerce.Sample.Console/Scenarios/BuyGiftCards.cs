namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Extensions;
    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Sample.Console;
    using Sitecore.Commerce.ServiceProxy;

    public static class BuyGiftCards
    {
        public static string ScenarioName = "BuyGiftCards";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                // First retrieve GiftCard as a SellableItem
                Proxy.GetValue(container.SellableItems.ByKey("Adventure Works Catalog,22565422120,100").Expand("Components($expand=ChildComponents($expand=ChildComponents))"));

                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|22565422120|100", 1));
                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|22565422120|050", 1));

                Proxy.DoCommand(
                    container.SetCartFulfillment(
                        cartId,
                        new ElectronicFulfillmentComponent
                        {
                            FulfillmentMethod = new EntityReference
                            {
                                EntityTarget = "8A23234F-8163-4609-BD32-32D9DD6E32F5",
                                Name = "Email"
                            },
                            EmailAddress = "g@g.com",
                            EmailContent = "this is the content of the email"
                        }));

                // Add a Payment
                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(150);
                Proxy.DoCommand(container.AddFederatedPayment(cartId, paymentComponent));

                var order = Orders.CreateAndValidateOrder(container, cartId, context);
                order.Status.Should().NotBe("Problem");
                order.Totals.GrandTotal.Amount.Should().Be(150.00000M);

                Proxy.GetEntityView(container, order.Id, "Master", string.Empty, string.Empty);

                watch.Stop();
                Console.WriteLine($"End {ScenarioName} (${order.Totals.GrandTotal.Amount}):{watch.ElapsedMilliseconds} ms");

                return Task.FromResult(order.Id);
            }
            catch (Exception ex)
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"Exception in Scenario {ScenarioName} (${ex.Message}) : Stack={ex.StackTrace}");
                return null;
            }
        }
    }
}
