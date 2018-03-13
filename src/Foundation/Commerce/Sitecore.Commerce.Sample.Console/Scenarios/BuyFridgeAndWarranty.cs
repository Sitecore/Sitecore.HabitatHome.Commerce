namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Extensions;
    using FluentAssertions;
    using Plugin.Fulfillment;
    using Plugin.Payments;
    using Console;
    using ServiceProxy;
    using Plugin.Carts;

    public static class BuyFridgeAndWarranty
    {
        public static string ScenarioName = "BuyFridgeAndWarranty";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                //Fridge - 
                var fridgeLine = Proxy.DoCommand(container.AddCartLine(cartId, "Habitat_Master|6042567|56042568", 1));

                //Microwave
                var microwaveLine = Proxy.DoCommand(container.AddCartLine(cartId, "Habitat_Master|6042757|56042758", 1));

                //3-year warranty
                var warrantyLine = Proxy.DoCommand(container.AddCartLine(cartId, "Habitat_Master|7042259|57042259", 1));

                //HealthTracker
                var healthTrackerLine = Proxy.DoCommand(container.AddCartLine(cartId, "Habitat_Master|6042886|56042887", 1));
                
                var fridgeFulfillmentResponse = Proxy.DoCommand(container.SetCartLineFulfillment(cartId, 
                    fridgeLine.Models.OfType<LineAdded>().FirstOrDefault().LineId, context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                var microwaveLineAdded = microwaveLine.Models.OfType<LineAdded>().FirstOrDefault();

                var microwaveFulfillmentResponse = Proxy.DoCommand(container.SetCartLineFulfillment(cartId,
                    microwaveLineAdded.LineId, context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                Proxy.DoCommand(
                    container.SetCartLineFulfillment(
                        cartId,
                        warrantyLine.Models.OfType<LineAdded>().FirstOrDefault().LineId,
                        context.Components.OfType<ElectronicFulfillmentComponent>().First()
                        )
                );

                var healthTrackerFulfillmentResponse = Proxy.DoCommand(container.SetCartLineFulfillment(cartId,
                    healthTrackerLine.Models.OfType<LineAdded>().FirstOrDefault().LineId, context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                var cart = Carts.GetCart(cartId, context);
                cart.Should().NotBeNull();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = cart.Totals.GrandTotal;
                Proxy.DoCommand(container.AddFederatedPayment(cartId, paymentComponent));
                
                var order = Orders.CreateAndValidateOrder(container, cartId, context);
                order.Status.Should().NotBe("Problem");
                order.Totals.GrandTotal.Amount.Should().Be(cart.Totals.GrandTotal.Amount);

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
