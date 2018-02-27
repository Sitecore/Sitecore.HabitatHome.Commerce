namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Extensions;
    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Sample.Console;
    using Sitecore.Commerce.ServiceProxy;

    public static class BuyWarranty
    {
        public static string ScenarioName = "BuyWarranty";

        public static Task<string> Run(ShopperContext context, decimal quantity)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                Proxy.DoCommand(container.AddCartLine(cartId, "Habitat_Master|7042259|57042259", quantity));

                var result = Proxy.DoCommand(
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

                var totals = result.Models.OfType<Sitecore.Commerce.Plugin.Carts.Totals>().FirstOrDefault();
                totals.Should().NotBeNull();
                totals?.GrandTotal.Should().NotBeNull();
                totals?.GrandTotal.Amount.Should().NotBe(0);

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount);
                result = Proxy.DoCommand(container.AddFederatedPayment(cartId, paymentComponent));
                totals = result.Models.OfType<Sitecore.Commerce.Plugin.Carts.Totals>().FirstOrDefault();
                totals.Should().NotBeNull();
                totals?.GrandTotal.Should().NotBeNull();
                totals?.GrandTotal.Amount.Should().NotBe(0);
                totals?.PaymentsTotal.Should().NotBeNull();
                totals?.PaymentsTotal.Amount.Should().NotBe(0);

                var order = Orders.CreateAndValidateOrder(container, cartId, context);
                order.Status.Should().NotBe("Problem");
                order.Totals.GrandTotal.Amount.Should().Be(totals.GrandTotal.Amount);

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
