namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Core;
    using Extensions;
    using Plugin.Carts;
    using Plugin.Fulfillment;
    using Plugin.Payments;

    using Sitecore.Commerce.Sample.Console;
    using Sitecore.Commerce.ServiceProxy;

    public static class BuySplitShipment
    {
        public static string ScenarioName = "BuySplitShipment";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                // Add Cart Line with Variant
                var commandResult = Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW098 04|5", 1) );
                var cartLineId1 = commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;

                // Add Cart Line without Variant
                commandResult = Proxy.DoCommand(
                    container.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1));
                var cartLineId2 = commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;

                commandResult = Proxy.DoCommand(
                    container.SetCartLineFulfillment(
                        cartId, 
                        cartLineId1,
                        context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                var totals = commandResult.Models.OfType<Totals>().First();

                commandResult = Proxy.DoCommand(
                    container.SetCartLineFulfillment(
                        cartId, 
                        cartLineId2,
                        context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                totals = commandResult.Models.OfType<Totals>().First();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount - totals.PaymentsTotal.Amount);
                Proxy.DoCommand(container.AddFederatedPayment(cartId, paymentComponent));

                var order = Orders.CreateAndValidateOrder(container, cartId, context);

                watch.Stop();

                if (order.Totals.GrandTotal.Amount != 180.40M)
                {
                    ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"BuySplitShipment - Expected GrandTotal:{180.40M} Actual:{order.Totals.GrandTotal.Amount}");
                }

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
