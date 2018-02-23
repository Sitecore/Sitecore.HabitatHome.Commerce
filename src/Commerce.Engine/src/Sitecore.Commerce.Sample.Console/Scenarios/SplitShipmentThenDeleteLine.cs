namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using EntityViews;
    using Extensions;

    using Sitecore.Commerce.Sample.Console;

    public static class SplitShipmentThenDeleteLine
    {
        public static string ScenarioName = "SplitShipmentThenDeleteLine";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var orderId = BuySplitShipment.Run(context).Result;
                if (!string.IsNullOrEmpty(orderId))
                {
                    OrdersUX.HoldOrder(orderId);
                }

                var order = Orders.GetOrder(container, orderId);

                // order.Totals.GrandTotal.Amount.Should().Be(187.03000M);
                if (order.Totals.GrandTotal.Amount != 180.40M)
                {
                    ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"GrandTotal Incorrect - Expecting:{180.40M} Actual:{order.Totals.GrandTotal.Amount}");
                }
                var lineToDelete = order.Lines.First();

                var action = new EntityView { Action = "DeleteLineItem", EntityId = orderId, ItemId = lineToDelete.Id };
                container.DoAction(action).GetValue();

                watch.Stop();

                Console.WriteLine($"End {ScenarioName} - {watch.ElapsedMilliseconds} ms");

                return Task.FromResult(orderId);
            }
            catch (Exception ex)
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"Exception in Scenario {ScenarioName} (${ex.Message}) : Stack={ex.StackTrace}");
                return null;
            }
        }

    }
}
