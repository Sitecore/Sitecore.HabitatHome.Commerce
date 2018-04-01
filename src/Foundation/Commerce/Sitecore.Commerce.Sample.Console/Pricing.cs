namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Extensions;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Pricing
    {
        private static string _priceBookFriendlyId;
        private static string _priceSnapshotId;
        private static string _priceTierId;
        private static string _priceCardFriendlyId;
        private static string _duplicatedPriceCardFriendlyId;
        private static readonly Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public  static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin PricingBooksAndCards");

            AddPriceBook();
            GetPriceBook();
            EditPriceBook();

            DisassociatedCatalogFromBook();
            AssociatedCatalogToBook();
            GetBookAssociatedCatalogs();

            AddPriceCard();
            EditPriceCard();
            DuplicatePriceCard();

            AddPriceSnapshot();
            EditPriceSnapshot();
            SetSnapshotApprovalStatus();

            AddPriceTier();
            EditPriceTier();

            AddPriceSnapshotTag();

            GetPriceCard();

            RemovePriceTier();
            RemovePriceSnapshotTag();
            RemovePriceSnapshot();

            DeletePriceCard();

            watch.Stop();

            Console.WriteLine($"End PricingBooksAndCards :{watch.ElapsedMilliseconds} ms");
        }
        
        public static PriceCard GetPriceCard(string cardFriendlyId = "")
        {
            Console.WriteLine("Begin GetPriceCard");

            var friendlyId = string.IsNullOrEmpty(cardFriendlyId)
                                ? _priceCardFriendlyId
                                : cardFriendlyId;

            var result = Proxy.GetValue(ShopsContainer.PriceCards.ByKey(friendlyId).Expand("Snapshots($expand=SnapshotComponents),Components"));
            result.Should().NotBeNull();

            return result;
        }

        #region Books

        private static void AddPriceBook()
        {
            Console.WriteLine("Begin AddPriceBook");

            var result = Proxy.DoCommand(ShopsContainer.AddPriceBook("ConsolePriceBook", "displayname", "description", "{0F65742E-317F-44B0-A4DE-EBF06209E8EE}"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            var priceBookCreated = result.Models.OfType<PriceBookAdded>().FirstOrDefault();
            _priceBookFriendlyId = priceBookCreated?.PriceBookFriendlyId;

            result = Proxy.DoCommand(ShopsContainer.AddPriceBook("ConsolePriceBook", "displayname", "description", "{0F65742E-317F-44B0-A4DE-EBF06209E8EE}"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.AddPriceBook("ConsolePriceBook", "displayname", "description", "{0F65742E-317F-44B0-A4DE-EBF06209E8EE}"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.AddPriceBook("ConsolePriceBook1", string.Empty, string.Empty, string.Empty));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            
            result = Proxy.DoCommand(ShopsContainer.AddPriceBook("ConsolePriceBook2", "displayname", "description", "{0F65742E-317F-44B0-A4DE-EBF06209E8EE}"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            result = Proxy.DoCommand(ShopsContainer.AddPriceBook("ConsolePriceBook3", string.Empty, "description", "{0F65742E-317F-44B0-A4DE-EBF06209E8EE}"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            result = Proxy.DoCommand(ShopsContainer.AddPriceBook("ConsolePriceBook4", "displayname", string.Empty, "{0F65742E-317F-44B0-A4DE-EBF06209E8EE}"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            result = Proxy.DoCommand(ShopsContainer.AddPriceBook("ConsolePriceBook5", "displayname", "description", string.Empty));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void GetPriceBook()
        {
            Console.WriteLine("Begin GetPriceBook");

            var result = Proxy.GetValue(ShopsContainer.PriceBooks.ByKey(_priceBookFriendlyId).Expand("Components"));
            result.Should().NotBeNull();
        }

        private static void EditPriceBook()
        {
            Console.WriteLine("Begin EditPriceBook");

            var result = Proxy.DoCommand(ShopsContainer.EditPriceBook(_priceBookFriendlyId, "edited description", "edited diplay name", "{0F65742E-317F-44B0-A4DE-EBF06209E8EE}"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }
        
        private static void GetBookAssociatedCatalogs()
        {
            Console.WriteLine("Begin GetBookAssociatedCatalogs");

            var result = Proxy.Execute(ShopsContainer.GetPriceBookAssociatedCatalogs("AdventureWorksPriceBook")).ToList();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        private static void AssociatedCatalogToBook()
        {
            Console.WriteLine("Begin AssociatedCatalogToBook");

            var result = Proxy.DoCommand(ShopsContainer.AssociateCatalogToPriceBook("AdventureWorksPriceBook", "Adventure Works Catalog"));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }

        private static void DisassociatedCatalogFromBook()
        {
            Console.WriteLine("Begin AssociatedCatalogsToBook");

            var result = Proxy.DoCommand(ShopsContainer.DisassociateCatalogFromPriceBook("AdventureWorksPriceBook", "Adventure Works Catalog"));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }

        #endregion

        #region Cards 

        private static void AddPriceCard()
        {
            Console.WriteLine("Begin AddPriceCard");

            var result = Proxy.DoCommand(ShopsContainer.AddPriceCard(_priceBookFriendlyId, "ConsolePriceCard", "displayname", "description"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PriceCardAdded>().FirstOrDefault().Should().NotBeNull();
            _priceCardFriendlyId = result.Models.OfType<PriceCardAdded>().FirstOrDefault()?.PriceCardFriendlyId;

            result = Proxy.DoCommand(ShopsContainer.AddPriceCard(_priceBookFriendlyId, "ConsolePriceCard", "displayname", "description"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.AddPriceCard(_priceBookFriendlyId, "ConsolePriceCard1", string.Empty, "description"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            result = Proxy.DoCommand(ShopsContainer.AddPriceCard(_priceBookFriendlyId, "ConsolePriceCard2", "displayname", string.Empty));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }
        
        private static void EditPriceCard()
        {
            Console.WriteLine("Begin EditPriceCard");

            var result = Proxy.DoCommand(ShopsContainer.EditPriceCard(_priceCardFriendlyId, "edited display name", "edited description"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void DeletePriceCard()
        {
            Console.WriteLine("Begin DeletePriceCard");

            var result = Proxy.DoCommand(ShopsContainer.DeletePriceCard("InvalidCard"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.DeletePriceCard(_priceCardFriendlyId));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            result = Proxy.DoCommand(ShopsContainer.DeletePriceCard(_duplicatedPriceCardFriendlyId)); // DELETING CLONED CARD 
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void DuplicatePriceCard()
        {
            Console.WriteLine("Begin DuplicatePriceCard");

            var result = Proxy.DoCommand(ShopsContainer.DuplicatePriceCard("InvalidCard", "ConsolePriceCardDuplicate"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.DuplicatePriceCard(_priceCardFriendlyId, "ConsolePriceCardDuplicate"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            _duplicatedPriceCardFriendlyId = result.Models.OfType<PriceCardAdded>().FirstOrDefault()?.PriceCardFriendlyId;
        }

        #endregion

        #region Snapshots

        private static void AddPriceSnapshot()
        {
            Console.WriteLine("Begin AddPriceSnapshot");

            var result = Proxy.DoCommand(ShopsContainer.AddPriceSnapshot("InvalidCard",DateTimeOffset.Now));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            var snapshotDate = DateTimeOffset.Now;
            result = Proxy.DoCommand(ShopsContainer.AddPriceSnapshot(_priceCardFriendlyId, snapshotDate));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PriceSnapshotAdded>().FirstOrDefault().Should().NotBeNull();
            _priceSnapshotId = result.Models.OfType<PriceSnapshotAdded>().FirstOrDefault()?.PriceSnapshotId;

            var card = GetPriceCard();
            var snapshot = card?.Snapshots.FirstOrDefault(s => s.Id.EndsWith(_priceSnapshotId));
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault().Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("Draft");
            snapshot?.BeginDate.Should().BeCloseTo(snapshotDate, 1000);

            result = Proxy.DoCommand(ShopsContainer.AddPriceSnapshot(_priceCardFriendlyId, snapshotDate));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");
        }

        private static void EditPriceSnapshot()
        {
            Console.WriteLine("Begin EditPriceSnapshot");

            var beginDate = DateTimeOffset.Now;

            var result = Proxy.DoCommand(ShopsContainer.EditPriceSnapshot("InvalidCard", _priceSnapshotId, beginDate));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.EditPriceSnapshot(_priceCardFriendlyId, "InvalidSnapshot", beginDate));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.EditPriceSnapshot(_priceCardFriendlyId, _priceSnapshotId, beginDate));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            var card = GetPriceCard();
            var snapshot = card?.Snapshots.FirstOrDefault(s => s.Id.EndsWith(_priceSnapshotId));
            snapshot?.BeginDate.Should().BeCloseTo(beginDate, 1000);
        }

        private static void RemovePriceSnapshot()
        {
            Console.WriteLine("Begin RemovePriceSnapshot");

            var result = Proxy.DoCommand(ShopsContainer.RemovePriceSnapshot("InvalidCard", _priceSnapshotId));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.RemovePriceSnapshot(_priceCardFriendlyId, "InvalidSnapshot"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.RemovePriceSnapshot(_priceCardFriendlyId, _priceSnapshotId));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void SetSnapshotApprovalStatus()
        {
            Console.WriteLine("Begin RemovePriceSnapshot");

            var container = new CsrSheila().Context.ShopsContainer();

            var snapshotDate = DateTimeOffset.Now.AddDays(30);
            var result = Proxy.DoCommand(container.AddPriceSnapshot(_priceCardFriendlyId, snapshotDate));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PriceSnapshotAdded>().FirstOrDefault().Should().NotBeNull();
            var snapshotId = result.Models.OfType<PriceSnapshotAdded>().FirstOrDefault()?.PriceSnapshotId;

            result = Proxy.DoCommand(ShopsContainer.AddPriceTier(_priceCardFriendlyId, snapshotId, "USD", 3, 13));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PriceTierAdded>().FirstOrDefault().Should().NotBeNull();

            result = Proxy.DoCommand(container.SetPriceSnapshotsApprovalStatus(_priceCardFriendlyId, new List<string> { snapshotId }, "ReadyForApproval", "my comment"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }

        #endregion

        #region Tiers

        private static void AddPriceTier()
        {
            Console.WriteLine("Begin AddPriceTier");

            var result = Proxy.DoCommand(ShopsContainer.AddPriceTier("InvalidCard", _priceSnapshotId, "USD", 3, 13));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.AddPriceTier(_priceCardFriendlyId, "InvalidSnapshot", "USD", 3, 13));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.AddPriceTier(_priceCardFriendlyId, _priceSnapshotId, "USD", 3, 13));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PriceTierAdded>().FirstOrDefault().Should().NotBeNull();
            _priceTierId = result.Models.OfType<PriceTierAdded>().FirstOrDefault()?.PriceTierId;

            result = Proxy.DoCommand(ShopsContainer.AddPriceTier(_priceCardFriendlyId, _priceSnapshotId, "USD", 3, 13));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");
        }

        private static void EditPriceTier()
        {
            Console.WriteLine("Begin EditPriceTier");

            var result = Proxy.DoCommand(ShopsContainer.EditPriceTier("InvalidCard", _priceSnapshotId, _priceTierId, 13));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.EditPriceTier(_priceCardFriendlyId, "InvalidSnapshot", _priceTierId, 13));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.EditPriceTier(_priceCardFriendlyId, _priceSnapshotId, "InvalidTiers", 13));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.EditPriceTier(_priceCardFriendlyId, _priceSnapshotId, _priceTierId, 13));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void RemovePriceTier()
        {
            Console.WriteLine("Begin RemovePriceTier");

            var result = Proxy.DoCommand(ShopsContainer.RemovePriceTier(_priceCardFriendlyId, _priceSnapshotId, "InvalidTier"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.RemovePriceTier(_priceCardFriendlyId, "InvalidSnapshot", _priceTierId));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.RemovePriceTier(_priceCardFriendlyId, _priceSnapshotId, _priceTierId));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion

        #region Tags

        private static void AddPriceSnapshotTag()
        {
            Console.WriteLine("Begin AddPriceSnapshotTag");

            var result = Proxy.DoCommand(ShopsContainer.AddPriceSnapshotTag(_priceCardFriendlyId, _priceSnapshotId, "ThisIsATag"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            result = Proxy.DoCommand(ShopsContainer.AddPriceSnapshotTag(_priceCardFriendlyId, _priceSnapshotId, "ThisIsATag"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.AddPriceSnapshotTag("InvalidCard", _priceSnapshotId, "AnotherTag"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.AddPriceSnapshotTag(_priceCardFriendlyId, "InvalidSnapshot", "AnotherTag"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");
        }

        private static void RemovePriceSnapshotTag()
        {
            Console.WriteLine("Begin RemovePriceSnapshotTag");

            var result = Proxy.DoCommand(ShopsContainer.RemovePriceSnapshotTag("InvalidCard", _priceSnapshotId, "ThisIsATag"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.RemovePriceSnapshotTag(_priceCardFriendlyId, "InvalidSnapshot", "ThisIsATag"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.RemovePriceSnapshotTag(_priceCardFriendlyId, _priceSnapshotId, "InvalidTag"));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            result = Proxy.DoCommand(ShopsContainer.RemovePriceSnapshotTag(_priceCardFriendlyId, _priceSnapshotId, "ThisIsATag"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion
    }
}
