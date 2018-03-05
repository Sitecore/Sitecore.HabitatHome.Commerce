namespace Sitecore.Commerce.Sample.Console
{
    using System.Diagnostics;
    using System.Linq;
    using Core;
    using FluentAssertions;

    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Carts
    {
        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            System.Console.WriteLine("Begin Carts");

            MergeCarts_BothHasLines();
            MergeCarts_LinesRollup();
            MergeCarts_FromWithComponents();
            MergeCarts_ToWithComponents();

            watch.Stop();

            System.Console.WriteLine($"End Carts :{watch.ElapsedMilliseconds} ms");
        }

        private static void MergeCarts_BothHasLines()
        {
            System.Console.WriteLine("Begin MergeCarts_BothHasLines");

            var fromCart = Proxy.GetValue(ShopsContainer.Carts.ByKey("consolemergecart1"));
            AddCartLine(fromCart.Id, "Adventure Works Catalog|AW098 04|5", 1);
            fromCart = GetCart(fromCart.Id);

            var toCart = Proxy.GetValue(ShopsContainer.Carts.ByKey("consolemergecart2"));
            AddCartLine(toCart.Id, "Adventure Works Catalog|AW475 14|", 1);

            var commandResult = Proxy.DoCommand(ShopsContainer.MergeCarts(fromCart.Id, toCart.Id));
            var model = commandResult.Models.OfType<PersistedEntityModel>().FirstOrDefault();
            model.Should().NotBeNull();
            var mergedCartId = model.EntityId;
            var mergedCart = GetCart(mergedCartId);
            mergedCart.Should().NotBeNull();
            mergedCart.Lines.Should().NotBeEmpty();
            mergedCart.Lines.Count.Should().Be(2);
            fromCart.Lines.ToList().ForEach(fl =>
            {
                mergedCart.Lines.FirstOrDefault(ml => ml.ItemId.Equals(fl.ItemId) && ml.Quantity == fl.Quantity).Should().NotBeNull();
            });
            mergedCart.Components.Should().NotBeEmpty();

            DeleteCart(mergedCart.Id);
        }

        private static void MergeCarts_LinesRollup()
        {
            System.Console.WriteLine("Begin MergeCarts_LinesRollup");

            var fromCart = Proxy.GetValue(ShopsContainer.Carts.ByKey("consolemergecartlinesrollup1"));
            AddCartLine(fromCart.Id, "Adventure Works Catalog|AW098 04|5", 1);
            fromCart = GetCart(fromCart.Id);

            var toCart = Proxy.GetValue(ShopsContainer.Carts.ByKey("consolemergecartlinesrollup2"));
            AddCartLine(toCart.Id, "Adventure Works Catalog|AW098 04|5", 1);

            var commandResult = Proxy.DoCommand(ShopsContainer.MergeCarts(fromCart.Id, toCart.Id));
            var model = commandResult.Models.OfType<PersistedEntityModel>().FirstOrDefault();
            model.Should().NotBeNull();
            var mergedCartId = model.EntityId;
            var mergedCart = GetCart(mergedCartId);
            mergedCart.Should().NotBeNull();           
            mergedCart.Lines.Should().NotBeEmpty();
            mergedCart.Lines.Count.Should().Be(1);
            mergedCart.Lines.ToList().ForEach(ml =>
            {
                fromCart.Lines.FirstOrDefault(fl => fl.ItemId.Equals(ml.ItemId) && ml.Quantity == fl.Quantity * 2).Should().NotBeNull();
            });
            mergedCart.Components.Should().NotBeEmpty();

            DeleteCart(mergedCart.Id);
        }

        private static void MergeCarts_FromWithComponents()
        {
            System.Console.WriteLine("Begin MergeCarts_FromWithComponents");

            var fromCart = Proxy.GetValue(ShopsContainer.Carts.ByKey("consolemergecartfromwithcomponents1"));
            AddCartLine(fromCart.Id, "Adventure Works Catalog|AW098 04|5", 1);
            Coupons.AddCouponToCart(fromCart.Id, "RTRNC15P");
            fromCart = GetCart(fromCart.Id);

            var toCart = Proxy.GetValue(ShopsContainer.Carts.ByKey("consolemergecartfromwithcomponents2"));
            AddCartLine(toCart.Id, "Adventure Works Catalog|AW475 14|", 1);

            var commandResult = Proxy.DoCommand(ShopsContainer.MergeCarts(fromCart.Id, toCart.Id));
            var model = commandResult.Models.OfType<PersistedEntityModel>().FirstOrDefault();
            model.Should().NotBeNull();
            var mergedCartId = model.EntityId;
            var mergedCart = GetCart(mergedCartId);
            mergedCart.Should().NotBeNull();           
            mergedCart.Lines.Should().NotBeEmpty();
            mergedCart.Components.Should().NotBeEmpty();
            fromCart.Components.ToList().ForEach(fc =>
            {
                mergedCart.Components.ToList().FirstOrDefault(mc => mc.Id.Equals(fc.Id)).Should().NotBeNull();
            });

            DeleteCart(mergedCart.Id);
        }

        private static void MergeCarts_ToWithComponents()
        {
            System.Console.WriteLine("Begin MergeCarts_ToWithComponents");

            var fromCart = Proxy.GetValue(ShopsContainer.Carts.ByKey("consolemergecarttowithcomponents1"));
            AddCartLine(fromCart.Id, "Adventure Works Catalog|AW098 04|5", 1);
            fromCart = GetCart(fromCart.Id);

            var toCart = Proxy.GetValue(ShopsContainer.Carts.ByKey("consolemergecarttowithcomponents2"));
            AddCartLine(toCart.Id, "Adventure Works Catalog|AW475 14|", 1);
            Coupons.AddCouponToCart(toCart.Id, "RTRNC15P");

            var commandResult = Proxy.DoCommand(ShopsContainer.MergeCarts(fromCart.Id, toCart.Id));
            var model = commandResult.Models.OfType<PersistedEntityModel>().FirstOrDefault();
            model.Should().NotBeNull();
            var mergedCartId = model.EntityId;
            var mergedCart = GetCart(mergedCartId);
            mergedCart.Should().NotBeNull();            
            mergedCart.Lines.Should().NotBeEmpty();
            mergedCart.Components.Should().NotBeEmpty();
            toCart.Components.ToList().ForEach(fc =>
            {
                mergedCart.Components.ToList().FirstOrDefault(mc => mc.Id.Equals(fc.Id)).Should().NotBeNull();
            });
            mergedCart.Components.ToList().OfType<CartCouponsComponent>().FirstOrDefault().Should().NotBeNull();

            DeleteCart(mergedCart.Id);
        }

        #region Helpers

        public static Cart GetCart(string cartId, ShopperContext context = null)
        {
            var container = context != null ? context.ShopsContainer() : ShopsContainer;
            var cart = Proxy.GetValue(container.Carts.ByKey(cartId).Expand("Lines($expand=CartLineComponents),Components"));

            return cart;
        }

        public static string AddCartLineWithVariant(string cartId)
        {
            var commandResult = Proxy.DoCommand(
                ShopsContainer.AddCartLine(cartId, "Adventure Works Catalog|AW098 04|5", 1));

            return commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;
        }

        public static string AddCartLineWithoutVariant(string cartId)
        {
            var commandResult = Proxy.DoCommand(
                ShopsContainer.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1));

            return commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;
        }

        public static string AddCartLine(string cartId, string itemId, decimal quantity)
        {
            var commandResult = Proxy.DoCommand(
                ShopsContainer.AddCartLine(cartId, itemId, quantity));

            return commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;
        }

        public static string AddCartLineGiftCard(string cartId)
        {
            var commandResult = Proxy.DoCommand(
                ShopsContainer.AddCartLine(cartId, "Adventure Works Catalog|22565422120|100", 1));

            return commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;
        }

        public static string AddCartLineGiftCard50(string cartId)
        {
            var commandResult = Proxy.DoCommand(
                ShopsContainer.AddCartLine(cartId, "Adventure Works Catalog|22565422120|050", 1));

            return commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;
        }

        public static void UpdateCartLine(string cartId, string lineId)
        {
            Proxy.DoCommand(
                ShopsContainer.UpdateCartLine(cartId, lineId, 10));
        }

        public static void DeleteCart(string cartId)
        {
            var cart = ShopsContainer.Carts.Where(p => p.Id == cartId).SingleOrDefault();
            ShopsContainer.DeleteObject(cart);
            ShopsContainer.SaveChanges();
        }
        
        public static void DisplayCartLine(CartLineComponent cartLine)
        {
            System.Console.WriteLine($"\t Itemid: {cartLine.ItemId}");
            System.Console.WriteLine($"\t Qty: {cartLine.Quantity} List:{cartLine.UnitListPrice.Amount}  Sell:{cartLine.Policies.OfType<PurchaseOptionMoneyPolicy>().FirstOrDefault()?.SellPrice.Amount}");
            System.Console.WriteLine($"\t SubTotal: {cartLine.Totals.SubTotal.Amount} Adj:{cartLine.Totals.AdjustmentsTotal.Amount}  Grand:{cartLine.Totals.GrandTotal.Amount}");
            System.Console.WriteLine();

            if (cartLine.Adjustments.Count > 0)
            {
                System.Console.WriteLine("\t \t Line adjustments:");
                foreach (var adjustment in cartLine.Adjustments)
                {
                    System.Console.WriteLine($"\t \t \t Name: {adjustment.Name} \t Amount: {adjustment.Adjustment.Amount} ");
                }
            }
            else
            {
                System.Console.WriteLine("\t \t (NO Line adjustments)");
            }

            System.Console.WriteLine("\t \t Line components:");
            foreach (var component in cartLine.CartLineComponents)
            {
                System.Console.WriteLine($"\t \t \t {component.GetType().Name}");
            }

            System.Console.WriteLine();
        }

        #endregion
    }
}
