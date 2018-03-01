namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using Extensions;
    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class PricingUX
    {
        private const string BookId = "Entity-PriceBook-ConsoleUxPriceBook";
        private const string CardId = "Entity-PriceCard-ConsoleUxPriceBook-ConsoleUxPriceCard";
        private static string _priceSnapshotId;
        private static string _priceCardFriendlyId;

        private static readonly Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin PricingUX");

            AddPriceBook();
            EditPriceBook();

            Books();
            BookMaster();
            BookDetails();
            
            DisassociateCatalog();
            AssociateCatalog();
            BookCatalogs();
            
            AddPriceCard();
            BookCards();
            CardMaster();
            CardDetails();
            EditPriceCard();
            DuplicatePriceCard();
            
            AddPriceSnapshot();
            CardSnapshots();
            EditPriceSnapshot();
            EditPriceSnapshot_WithTags();
            SetSnapshotApprovalStatus();

            AddCurrency();
            EditCurrency();
            RemoveCurrency();

            RemovePriceSnapshot();
            DeletePriceCard();

            watch.Stop();

            Console.WriteLine($"End PricingUX :{watch.ElapsedMilliseconds} ms");
        }

        #region Books

        private static void Books()
        {
            Console.WriteLine("Begin PriceBooks View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(string.Empty, "PriceBooks", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().BeEmpty();
            result.ChildViews.Should().NotBeEmpty();

            foreach (var childView in result.ChildViews.Cast<EntityView>())
            {
                childView.Should().NotBeNull();
                childView.Properties.Should().NotBeEmpty();
                childView.Policies.Should().BeEmpty();
                childView.ChildViews.Should().BeEmpty();
            }
        }

        private static void BookMaster()
        {
            Console.WriteLine("Begin BookMaster View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(BookId, "Master", string.Empty, string.Empty));

            result.Should().NotBeNull();
            result.Policies.Should().BeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();

            foreach (var childView in result.ChildViews.Cast<EntityView>())
            {
                childView.Should().NotBeNull();
                childView.Policies.Should().NotBeEmpty();
            }
        }

        private static void BookDetails()
        {
            Console.WriteLine("Begin BookDetails View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(BookId, "Details", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static void BookCards()
        {
            Console.WriteLine("Begin Books Cards View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(BookId, "PriceBookCards", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();

            foreach (var childView in result.ChildViews.Cast<EntityView>())
            {
                childView.Should().NotBeNull();
                childView.Properties.Should().NotBeEmpty();
                childView.Policies.Should().BeEmpty();
                childView.ChildViews.Should().BeEmpty();
            }
        }

        private static void BookCatalogs()
        {
            Console.WriteLine("Begin Books Catalogs View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(BookId, "PriceBookCatalogs", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();

            foreach (var childView in result.ChildViews.Cast<EntityView>())
            {
                childView.Should().NotBeNull();
                childView.Properties.Should().NotBeEmpty();
                childView.Policies.Should().BeEmpty();
                childView.ChildViews.Should().BeEmpty();
            }
        }

        private static void AddPriceBook()
        {
            Console.WriteLine("Begin AddPriceBookUX");
            
            var view = Proxy.GetValue(ShopsContainer.GetEntityView(string.Empty, "Details", "AddPriceBook", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = "InvalidPriceBook{" },
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = "ConsoleUxPriceBook" },
                                      new ViewProperty { Name = "DisplayName", Value = "displayname" },
                                      new ViewProperty { Name = "Description", Value = "description" },
                                      new ViewProperty
                                          {
                                              Name = "CurrencySetId",
                                              Value = "{0F65742E-317F-44B0-A4DE-EBF06209E8EE}"
                                          }
                                  };
            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void EditPriceBook()
        {
            Console.WriteLine("Begin EditPriceBookUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(BookId, "Details", "EditPriceBook", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Description", Value = "edited description" },
                                      new ViewProperty { Name = "DisplayName", Value = "edited display name" },
                                      new ViewProperty { Name = "CurrencySetId", Value = "{0F65742E-317F-44B0-A4DE-EBF06209E8EE}" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void AssociateCatalog()
        {
            Console.WriteLine("Begin AssociateCatalogUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PriceBook-AdventureWorksPriceBook", "PriceBookCatalogs", "AssociateCatalog", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            foreach (var property in view.Properties)
            {
                property.Name.Should().NotBeNullOrEmpty();
                if (property.Name.Equals("CatalogName"))
                {
                    property.Policies.Should().NotBeEmpty();
                }
                else
                {
                    property.Policies.Should().BeEmpty();
                }
            }

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "CatalogName", Value = "Adventure Works Catalog" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void DisassociateCatalog()
        {
            Console.WriteLine("Begin DisassociateCatalogUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PriceBook-AdventureWorksPriceBook", "Details", string.Empty, string.Empty));
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view = new EntityView
            {
                Action = "DisassociateCatalog",
                Name = string.Empty,
                EntityId = "Entity-PriceBook-AdventureWorksPriceBook",
                ItemId = "Adventure Works Catalog",
                Properties = new ObservableCollection<ViewProperty>
                                 {
                                     version
                                 }
                };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion

        #region Cards

        private static void CardMaster()
        {
            Console.WriteLine("Begin CardMaster View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PriceCard-AdventureWorksPriceBook-AdventureWorksPriceCard", "Master", string.Empty, string.Empty));

            result.Should().NotBeNull();
            result.Policies.Should().BeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
        }

        private static void CardSnapshots()
        {
            Console.WriteLine("Begin CardSnapshots View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PriceCard-AdventureWorksPriceBook-AdventureWorksPriceCard", "PriceCardSnapshots", string.Empty, string.Empty));

            result.Should().NotBeNull();
            result.EntityId.Should().NotBeNullOrEmpty();
            result.ItemId.Should().BeNullOrEmpty();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
        }

        private static void CardDetails()
        {
            Console.WriteLine("Begin CardDetails View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PriceCard-AdventureWorksPriceBook-AdventureWorksPriceCard", "Details", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static void AddPriceCard()
        {
            Console.WriteLine("Begin AddPriceCardUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(BookId, "Details", "AddPriceCard", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = "InvalidPriceCard^" },
                                      new ViewProperty { Name = "BookName", Value = "ConsoleUxPriceBook" },
                                      //new ViewProperty { Name = "Description", Value = "card's description" },
                                      //new ViewProperty { Name = "DisplayName", Value = "card's display name" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = "ConsoleUxPriceCard" },
                                      new ViewProperty { Name = "BookName", Value = "ConsoleUxPriceBook" },
                                      new ViewProperty { Name = "Description", Value = "card's description" },
                                      new ViewProperty { Name = "DisplayName", Value = "card's display name" },
                                      version
                                  };
            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PriceCardAdded>().FirstOrDefault().Should().NotBeNull();
            _priceCardFriendlyId = result.Models.OfType<PriceCardAdded>().FirstOrDefault()?.PriceCardFriendlyId;
        }

        private static void EditPriceCard()
        {
            Console.WriteLine("Begin EditPriceCardUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "Details", "EditPriceCard", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Description", Value = "edited description" },
                                      new ViewProperty { Name = "DisplayName", Value = "edited display anem" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void DuplicatePriceCard()
        {
            Console.WriteLine("Begin DuplicatePriceCardUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "Details", "DuplicatePriceCard", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "DuplicateCardName", Value = "ConsoleUxPriceCardDuplicate" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            // DELETING CLONE CARD
            result = Proxy.DoCommand(ShopsContainer.DeletePriceCard("Entity-PriceCard-ConsoleUxPriceBook-ConsoleUxPriceCardDuplicate"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void DeletePriceCard()
        {
            Console.WriteLine("Begin DeletePriceCardUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(BookId, "Details", string.Empty, string.Empty));
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Action = "DeletePriceCard";
            view.ItemId = CardId;
            view.Properties = new ObservableCollection<ViewProperty> { version };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion

        #region Snapshots

        private static void AddPriceSnapshot()
        {
            Console.WriteLine("Begin AddPriceSnapshotUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "PriceSnapshotDetails", "AddPriceSnapshot", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            var snapshotDate = DateTimeOffset.Now;
            view.Properties = new ObservableCollection<ViewProperty>
                {
                    new ViewProperty { Name = "BeginDate", Value = snapshotDate.ToString(CultureInfo.InvariantCulture) },
                    version
                };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PriceSnapshotAdded>().FirstOrDefault().Should().NotBeNull();
            _priceSnapshotId = result.Models.OfType<PriceSnapshotAdded>().FirstOrDefault()?.PriceSnapshotId;
            var card = Proxy.GetValue(ShopsContainer.PriceCards.ByKey(_priceCardFriendlyId).Expand("Snapshots($expand=SnapshotComponents),Components"));
            card.Should().NotBeNull();
            var snapshot = card?.Snapshots.FirstOrDefault(s => s.Id.EndsWith(_priceSnapshotId));
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault().Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("Draft");
            snapshot?.BeginDate.Should().BeCloseTo(snapshotDate, 1000);
        }

        private static void EditPriceSnapshot()
        {
            Console.WriteLine("Begin EditPriceSnapshotUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "PriceSnapshotDetails", "EditPriceSnapshot", _priceSnapshotId));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            var beginDate = DateTimeOffset.Now.AddDays(30);
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty
                                          {
                                              Name = "BeginDate",
                                              Value = beginDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      version
                                  };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            var card = Proxy.GetValue(ShopsContainer.PriceCards.ByKey(_priceCardFriendlyId).Expand("Snapshots($expand=SnapshotComponents),Components"));
            card.Should().NotBeNull();
            var snapshot = card?.Snapshots.FirstOrDefault(s => s.Id.EndsWith(_priceSnapshotId));
            snapshot?.BeginDate.Should().BeCloseTo(beginDate, 1000);
        }

        private static void EditPriceSnapshot_WithTags()
        {
            Console.WriteLine("Begin EditPriceSnapshotUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "PriceSnapshotDetails", "EditPriceSnapshot", _priceSnapshotId));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();          

            var beginDate = DateTimeOffset.Now.AddDays(30);
            view.Properties.FirstOrDefault(p => p.Name.Equals("BeginDate")).Value = beginDate.ToString(CultureInfo.InvariantCulture);
            view.Properties.FirstOrDefault(p => p.Name.Equals("IncludedTags")).Value = "['IncludedTag1', 'IncludedTag2']";

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            var card = Proxy.GetValue(ShopsContainer.PriceCards.ByKey(_priceCardFriendlyId).Expand("Snapshots($expand=SnapshotComponents),Components"));
            card.Should().NotBeNull();
            var snapshot = card?.Snapshots.FirstOrDefault(s => s.Id.EndsWith(_priceSnapshotId));
            snapshot?.BeginDate.Should().BeCloseTo(beginDate, 1000);
            snapshot?.Tags.Should().NotBeNullOrEmpty();
            snapshot?.Tags.Count.Should().Be(2);
        }

        private static void RemovePriceSnapshot()
        {
            Console.WriteLine("Begin RemovePriceSnapshotUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "PriceSnapshotDetails", string.Empty, _priceSnapshotId));
            view.Should().NotBeNull();
            view.Policies.Should().NotBeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().NotBeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Action = "RemovePriceSnapshot";
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void SetSnapshotApprovalStatus()
        {
            Console.WriteLine("Begin SetSnapshotApprovalStatusUX");

            var container = new CsrSheila().Context.ShopsContainer();

            var result = Proxy.DoCommand(container.AddPriceCard(BookId, "consoleapprovalpricecard", "displayname", "description"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            var cardFriendlyId = result.Models.OfType<PriceCardAdded>().FirstOrDefault()?.PriceCardFriendlyId;
            cardFriendlyId.Should().NotBeNullOrEmpty();
            var cardId = $"Entity-PriceCard-{cardFriendlyId}";

            var snapshotDate = DateTimeOffset.Now.AddDays(3);
            result = Proxy.DoCommand(container.AddPriceSnapshot(cardId, snapshotDate));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            var snapshotId = result.Models.OfType<PriceSnapshotAdded>().FirstOrDefault()?.PriceSnapshotId;

            result = Proxy.DoCommand(ShopsContainer.AddPriceTier(cardId, snapshotId, "USD", 3, 13));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PriceTierAdded>().FirstOrDefault().Should().NotBeNull();

            // REQUEST APPROVAL
            var view = Proxy.GetValue(container.GetEntityView(cardId, "SetSnapshotApprovalStatus", "RequestSnapshotApproval", snapshotId));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "request approval comment";
            result = Proxy.DoCommand(container.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            var card = Pricing.GetPriceCard(cardFriendlyId);
            var snapshot = card.Snapshots.FirstOrDefault(s => s.Id.Equals(snapshotId, StringComparison.OrdinalIgnoreCase));
            snapshot.Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault().Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("ReadyForApproval");

            // REJECT
            view = Proxy.GetValue(container.GetEntityView(cardId, "SetSnapshotApprovalStatus", "RejectSnapshot", snapshotId));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "reject comment";
            result = Proxy.DoCommand(container.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            card = Pricing.GetPriceCard(cardFriendlyId);
            snapshot = card.Snapshots.FirstOrDefault(s => s.Id.Equals(snapshotId, StringComparison.OrdinalIgnoreCase));
            snapshot.Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault().Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("Draft");

            view = Proxy.GetValue(ShopsContainer.GetEntityView(cardId, "SetSnapshotApprovalStatus", "RequestSnapshotApproval", snapshotId));
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "request approval second time comment";
            result = Proxy.DoCommand(container.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            
            // APPROVE
            view = Proxy.GetValue(container.GetEntityView(cardId, "SetSnapshotApprovalStatus", "ApproveSnapshot", snapshotId));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "approve comment";
            result = Proxy.DoCommand(container.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            card = Pricing.GetPriceCard(cardFriendlyId);
            snapshot = card.Snapshots.FirstOrDefault(s => s.Id.Equals(snapshotId, StringComparison.OrdinalIgnoreCase));
            snapshot.Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault().Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("Approved");

            // RETRACT
            view = Proxy.GetValue(container.GetEntityView(cardId, "SetSnapshotApprovalStatus", "RetractSnapshot", snapshotId));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "retract comment";
            result = Proxy.DoCommand(container.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            card = Pricing.GetPriceCard(cardFriendlyId);
            snapshot = card.Snapshots.FirstOrDefault(s => s.Id.Equals(snapshotId, StringComparison.OrdinalIgnoreCase));
            snapshot.Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault().Should().NotBeNull();
            snapshot?.SnapshotComponents.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("Draft");
        }

        #endregion

        #region Tiers

        private static void AddCurrency()
        {
            Console.WriteLine("Begin AddCurrency");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "PriceRow", "SelectCurrency", $"{_priceSnapshotId}"));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            view.Properties.FirstOrDefault(p => p.Name.Equals("Currency")).Value = "USD";
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            view = result.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(view.Name));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view?.ChildViews.Should().NotBeEmpty();
            view?.Action.Should().Be("AddCurrency");

            ((EntityView)view.ChildViews.FirstOrDefault()).Properties.FirstOrDefault(p => p.Name.Equals("Quantity")).Value = "1";
            ((EntityView)view.ChildViews.FirstOrDefault()).Properties.FirstOrDefault(p => p.Name.Equals("Price")).Value = "20";
            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PriceTierAdded>().FirstOrDefault().Should().NotBeNull();

            view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "PriceRow", string.Empty, $"{_priceSnapshotId}|USD"));
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();
        }

        private static void EditCurrency()
        {
            Console.WriteLine("Begin EditCurrency");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "PriceRow", "EditCurrency", $"{_priceSnapshotId}|USD"));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().NotBeEmpty();
            
            ((EntityView)view.ChildViews.FirstOrDefault()).Properties.FirstOrDefault(p => p.Name.Equals("Price")).Value = "200";
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "PriceRow", string.Empty, $"{_priceSnapshotId}|USD"));
            view.Should().NotBeNull();
            view.Properties.FirstOrDefault(p => p.Name.Equals("1")).Should().NotBeNull();
            view.Properties.FirstOrDefault(p => p.Name.Equals("1"))?.Value.Should().Be("200");
        }

        private static void RemoveCurrency()
        {
            Console.WriteLine("Begin RemoveCurrency");
            
            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CardId, "PriceRow", string.Empty, $"{_priceSnapshotId}|USD"));

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Action = "RemoveCurrency";
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      version
                                  };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion
    }
}
