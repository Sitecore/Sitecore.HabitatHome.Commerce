namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Extensions;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class PromotionsUX
    {
        private const string BookName = "consoleuxpromotionbook";
        private const string ItemId = "Entity-SellableItem-AW055 01|33";
        private const string Catalog = "Adventure Works Catalog";
        private const string CompleteItem = "Adventure Works Catalog|AW055 01|33";

        private static string _bookId;

        private static readonly Sitecore.Commerce.Engine.Container ShopsContainer = new CsrSheila().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin PromotionsUX");

            AddPromotionBook();
            EditPromotionBook();

            Books();
            BookMaster();
            BookDetails();

            DisassociateCatalog();
            AssociateCatalog();
            BookCatalogs();

            var promotionFriendlyId = AddPromotion("consoleuxpromotion");
            var promotionId = $"Entity-Promotion-{promotionFriendlyId}";
            ValidateAddPromotion("consoleuxpromotion");
            BookPromotions();
            PromotionMaster(promotionId);
            PromotionPreview(promotionId);
            PromotionDetails(promotionId);
            EditPromotion(promotionId, promotionFriendlyId);
            var duplicateFriendlyId = DuplicatePromotion(promotionId, "DuplicatePromotion");
            SetPromotionApprovalStatus();

            var qualificationId = AddQualification(promotionId);
            Qualifications(promotionId);
            QualificationDetails(promotionId, qualificationId);
            EditQualification(promotionId, qualificationId);

            var benefitId = AddBenefit(promotionId);
            Benefits(promotionId);
            BenefitDetails(promotionId, benefitId);
            EditBenefit(promotionId, benefitId);

            AddItem(promotionId, ItemId, Catalog);
            Items(promotionId);
            ItemDetails(promotionId, ItemId);

            RemoveItem(promotionId, CompleteItem);
            DeleteBenefit(promotionId, benefitId);
            DeleteQualification(promotionId, qualificationId);
            DeletePromotion(promotionId);
            DeletePromotion($"Entity-Promotion-{duplicateFriendlyId}");

            //// CreatePromotion();

            watch.Stop();

            Console.WriteLine($"End PromotionsUX :{watch.ElapsedMilliseconds} ms");
        }

        #region Books

        private static void Books()
        {
            Console.WriteLine("Begin PromotionBooks View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(string.Empty, "PromotionBooks", string.Empty, string.Empty));
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

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(_bookId, "Master", string.Empty, string.Empty));

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

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(_bookId, "Details", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static void AddPromotionBook()
        {
            Console.WriteLine("Begin AddPromotionBookUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(string.Empty, "Details", "AddPromotionBook", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = "InvalidBookName%(" },
                                      new ViewProperty { Name = "DisplayName", Value = "displayname" },
                                      new ViewProperty { Name = "Description", Value = "description" }
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = BookName },
                                      new ViewProperty { Name = "DisplayName", Value = "displayname" },
                                      new ViewProperty { Name = "Description", Value = "description" }
                                  };
            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            _bookId = $"Entity-PromotionBook-{result.Models.OfType<PromotionBookAdded>().FirstOrDefault()?.PromotionBookFriendlyId}";

            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");
        }

        private static void EditPromotionBook()
        {
            Console.WriteLine("Begin EditPromotionBookUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_bookId, "Details", "EditPromotionBook", string.Empty));
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

        private static void BookPromotions()
        {
            Console.WriteLine("Begin Books Promotions View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(_bookId, "PromotionBookPromotions", string.Empty, string.Empty));
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

            var result = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PromotionBook-AdventureWorksPromotionBook", "PromotionBookCatalogs", string.Empty, string.Empty));
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

        private static void AssociateCatalog()
        {
            Console.WriteLine("Begin PromotionAssociateCatalogUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PromotionBook-AdventureWorksPromotionBook", "PromotionBookCatalogs", "AssociateCatalog", string.Empty));
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
                                      new ViewProperty { Value = "Adventure Works Catalog", Name = "CatalogName" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void DisassociateCatalog()
        {
            Console.WriteLine("Begin PromotionDisassociateCatalogUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PromotionBook-AdventureWorksPromotionBook", "Details", string.Empty, string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().NotBeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Action = "DisassociateCatalog";
            view.ItemId = "Adventure Works Catalog";
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion

        #region Promotions

        private static void PromotionMaster(string promotionId)
        {
            Console.WriteLine("Begin PromotionMaster View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "Master", string.Empty, string.Empty));

            result.Should().NotBeNull();
            result.EntityId.Should().NotBeNullOrEmpty();
            result.ItemId.Should().BeNullOrEmpty();
            result.Policies.Should().BeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
        }

        private static void PromotionPreview(string promotionId)
        {
            Console.WriteLine("Begin PromotionPreview View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "Preview", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().BeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static void PromotionDetails(string promotionId)
        {
            Console.WriteLine("Begin PromotionDetails View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "Details", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static string AddPromotion(string name, DateTimeOffset? validFrom = null, DateTimeOffset? validTo = null)
        {
            Console.WriteLine("Begin AddPromotionUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_bookId, "Details", "AddPromotion", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            var fromDate = validFrom ?? DateTimeOffset.Now;
            var toDate = validTo ?? DateTimeOffset.Now.AddDays(30);
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = name },
                                      new ViewProperty { Name = "Description", Value = "promotion's description" },
                                      new ViewProperty { Name = "DisplayName", Value = "promotion's display name" },
                                      new ViewProperty { Name = "DisplayText", Value = "promotion's text" },
                                      new ViewProperty { Name = "DisplayCartText", Value = "promotion's cart text" },
                                      new ViewProperty
                                          {
                                              Name = "ValidFrom",
                                              Value = fromDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty
                                          {
                                              Name = "ValidTo",
                                              Value = toDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty { Name = "IsExclusive", Value = "true" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PromotionAdded>().FirstOrDefault().Should().NotBeNull();
            var promotionFriendlyId = result.Models.OfType<PromotionAdded>().FirstOrDefault()?.PromotionFriendlyId;

            var promotion = Promotions.GetPromotion(promotionFriendlyId);
            promotion.Should().NotBeNull();
            promotion.ValidFrom.Should().BeCloseTo(fromDate, 1000);
            promotion.ValidTo.Should().BeCloseTo(toDate, 1000);

            return promotionFriendlyId;
        }

        private static void ValidateAddPromotion(string name)
        {
            Console.WriteLine("Begin ValidateAddPromotionUX");
            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_bookId, "Details", "AddPromotion", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            var fromDate = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now.AddDays(30);

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = "'InvalidName~" },
                                      new ViewProperty { Name = "DisplayText", Value = "promotion's text" },
                                      new ViewProperty { Name = "DisplayCartText", Value = "promotion's cart text" },
                                      new ViewProperty
                                          {
                                              Name = "ValidFrom",
                                              Value = fromDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty
                                          {
                                              Name = "ValidTo",
                                              Value = toDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty { Name = "IsExclusive", Value = "true" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = name },
                                      new ViewProperty { Name = "Description", Value = "promotion's description" },
                                      new ViewProperty { Name = "DisplayName", Value = "promotion's display name" },
                                      new ViewProperty { Name = "DisplayText", Value = "promotion's text" },
                                      new ViewProperty { Name = "DisplayCartText", Value = "promotion's cart text" },
                                      new ViewProperty
                                          {
                                              Name = "ValidFrom",
                                              Value = fromDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty
                                          {
                                              Name = "ValidTo",
                                              Value = toDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty { Name = "IsExclusive", Value = "true" },
                                      version
                                  };
            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");
        }

        private static void EditPromotion(string promotionId, string promotionFriendlyId)
        {
            Console.WriteLine("Begin EditPromotionUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "Details", "EditPromotion", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            var fromDate = DateTimeOffset.Now.AddDays(60);
            var toDate = DateTimeOffset.Now.AddDays(90);
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty
                                          {
                                              Name = "ValidFrom",
                                              Value = fromDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty
                                          {
                                              Name = "ValidTo",
                                              Value = toDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty { Name = "DisplayText", Value = "edited text" },
                                      new ViewProperty { Name = "DisplayCartText", Value = "edited cart text" },
                                      new ViewProperty { Name = "Description", Value = "edited description" },
                                      new ViewProperty { Name = "DisplayName", Value = "edited display name" },
                                      new ViewProperty { Name = "IsExclusive", Value = "false" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            var promotion = Promotions.GetPromotion(promotionFriendlyId);
            promotion.Should().NotBeNull();
            promotion.ValidFrom.Should().BeCloseTo(fromDate, 1000);
            promotion.ValidTo.Should().BeCloseTo(toDate, 1000);
        }

        private static void DeletePromotion(string promotionId)
        {
            Console.WriteLine("Begin DeletePromotionUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_bookId, "Details", string.Empty, string.Empty));
            view.Should().NotBeNull();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Action = "DeletePromotion";
            view.ItemId = promotionId;
            view.Properties = new ObservableCollection<ViewProperty> { version };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static string DuplicatePromotion(string promotionId, string name)
        {
            Console.WriteLine("Begin DuplicatePromotionUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "Details", name, string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "PromotionDuplicateName", Value = "consoleuxduplicatepromotion" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            var friendlyId = result.Models.OfType<PromotionAdded>().FirstOrDefault()?.PromotionFriendlyId;
            friendlyId.Should().NotBeNullOrEmpty();

            return friendlyId;
        }

        private static void SetPromotionApprovalStatus()
        {
            Console.WriteLine("Begin SetPromotionApprovalStatusUX");

            // ADD PROMOTION TO BE APPROVE
            var promotionFriendlyId = AddPromotion("consoleuxpromotionapproved", DateTimeOffset.Now.AddDays(3), DateTimeOffset.Now.AddDays(30));
            promotionFriendlyId.Should().NotBeNullOrEmpty();
            var promotionId = $"Entity-Promotion-{promotionFriendlyId}";
            AddQualification(promotionId);
            AddBenefit(promotionId);

            ApprovePromotion(promotionId, promotionFriendlyId);

            // RETRACT
            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "SetPromotionApprovalStatus", "RetractPromotion", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "retract comment";

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            var promotion = Promotions.GetPromotion(promotionFriendlyId);
            promotion.Components.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("Draft");

            // DISABLE
            //// ADD PROMOTION TO BE DISABLE
            promotionFriendlyId = AddPromotion("consoleuxpromotiondisabled", DateTimeOffset.Now.AddDays(-3), DateTimeOffset.Now.AddDays(30));
            promotionFriendlyId.Should().NotBeNullOrEmpty();
            promotionId = $"Entity-Promotion-{promotionFriendlyId}";
            AddQualification(promotionId);
            AddBenefit(promotionId);

            ApprovePromotion(promotionId, promotionFriendlyId);

            view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "SetPromotionApprovalStatus", "DisablePromotion", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "disable comment";

            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            promotion = Promotions.GetPromotion(promotionFriendlyId);
            promotion.Components.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("Approved");
            promotion.Policies.OfType<DisabledPolicy>().FirstOrDefault().Should().NotBeNull();

            var detailsView = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "Details", string.Empty, string.Empty));
            detailsView.Should().NotBeNull();
            detailsView.Policies.Should().NotBeEmpty();
            detailsView.Policies.OfType<ActionsPolicy>().FirstOrDefault().Should().NotBeNull();
            detailsView.Policies.OfType<ActionsPolicy>().FirstOrDefault()?.Actions.FirstOrDefault(a => a.Name.Equals("DisablePromotion")).Should().NotBeNull();
            detailsView.Policies.OfType<ActionsPolicy>().FirstOrDefault()?.Actions.FirstOrDefault(a => a.Name.Equals("DisablePromotion"))?.IsEnabled.Should().BeFalse();
        }

        private static void ApprovePromotion(string promotionId, string promotionFriendlyId)
        {
            // REQUEST APPROVAL
            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "SetPromotionApprovalStatus", "RequestPromotionApproval", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "request approval comment";

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            var promotion = Promotions.GetPromotion(promotionFriendlyId);
            promotion.Components.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("ReadyForApproval");

            // REJECT
            view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "SetPromotionApprovalStatus", "RejectPromotion", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "reject comment";

            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            promotion = Promotions.GetPromotion(promotionFriendlyId);
            promotion.Components.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("Draft");

            //// REQUESTING APPROVAL AGAIN AFTER REJECTING THE PROMOTION
            view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "SetPromotionApprovalStatus", "RequestPromotionApproval", string.Empty));
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "request approval second time comment";
            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            promotion = Promotions.GetPromotion(promotionFriendlyId);
            promotion.Components.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("ReadyForApproval");

            // APPROVE
            view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "SetPromotionApprovalStatus", "ApprovePromotion", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Comment")).Value = "approve comment";

            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Messages.Any(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

            promotion = Promotions.GetPromotion(promotionFriendlyId);
            promotion.Components.OfType<ApprovalComponent>().FirstOrDefault()?.Status.Should().Be("Approved");
        }

        #endregion

        #region Qualifications

        private static void Qualifications(string promotionId)
        {
            Console.WriteLine("Begin Qualifications View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "Qualifications", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
        }

        private static void QualificationDetails(string promotionId, string qualificationId)
        {
            Console.WriteLine("Begin QualificationDetails View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "QualificationDetails", string.Empty, qualificationId));
            result.Should().NotBeNull();
            result.Policies.Should().BeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static string AddQualification(string promotionId)
        {
            Console.WriteLine("Begin AddQualificationUX");

            var selectView = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "QualificationDetails", "SelectQualification", string.Empty));
            selectView.Should().NotBeNull();
            selectView.Policies.Should().BeEmpty();
            selectView.Properties.Should().NotBeEmpty();
            selectView.ChildViews.Should().BeEmpty();

            var version = selectView.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            selectView.Properties = new ObservableCollection<ViewProperty>
                                        {
                                            new ViewProperty { Name = "Condition", Value = "CartHasFulfillmentCondition" },
                                            version
                                        };
            var selectAction = Proxy.DoCommand(ShopsContainer.DoAction(selectView));
            selectAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            var addView = selectAction.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(selectView.Name));
            addView.Should().NotBeNull();
            addView?.Policies.Should().BeEmpty();
            addView?.Properties.Should().NotBeEmpty();
            addView?.ChildViews.Should().BeEmpty();
            addView?.Action.Should().Be("AddQualification");
            version = addView?.Properties.FirstOrDefault(p => p.Name.Equals("Version"));
            addView.Properties = new ObservableCollection<ViewProperty>
                                     {
                                         new ViewProperty { Name = "Condition", Value = "CartHasFulfillmentCondition" },
                                         new ViewProperty { Name = "ConditionOperator", Value = "And" },
                                         version
                                     };
            var addAction = Proxy.DoCommand(ShopsContainer.DoAction(addView));
            addAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            var qualificationId = addAction.Models.OfType<QualificationAdded>().FirstOrDefault()?.QualificationId;
            qualificationId.Should().NotBeNullOrEmpty();

            return qualificationId;
        }

        private static void EditQualification(string promotionId, string qualificationId)
        {
            Console.WriteLine("Begin EditQualificationUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "QualificationDetails", "EditQualification", qualificationId));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view?.ChildViews.Should().BeEmpty();
            view?.Action.Should().Be("EditQualification");

            var version = view?.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Condition", Value = "CartHasFulfillmentCondition" },
                                      new ViewProperty { Name = "ConditionOperator", Value = "Or" },
                                      version
                                  };
            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void DeleteQualification(string promotionId, string qualificationId)
        {
            Console.WriteLine("Begin DeleteQualificationUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "QualificationDetails", "DeleteQualification", qualificationId));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion

        #region Benefits

        private static void Benefits(string promotionId)
        {
            Console.WriteLine("Begin Benefits View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "Benefits", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
        }

        private static void BenefitDetails(string promotionId, string benefitId)
        {
            Console.WriteLine("Begin BenefitDetails View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "BenefitDetails", string.Empty, benefitId));
            result.Should().NotBeNull();
            result.Policies.Should().BeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static string AddBenefit(string promotionId)
        {
            Console.WriteLine("Begin AddBenefitUX");

            var selectView = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "BenefitDetails", "SelectBenefit", string.Empty));
            selectView.Should().NotBeNull();
            selectView.Policies.Should().BeEmpty();
            selectView.Properties.Should().NotBeEmpty();
            selectView.ChildViews.Should().BeEmpty();
            var version = selectView?.Properties.FirstOrDefault(p => p.Name.Equals("Version"));
            selectView.Properties = new ObservableCollection<ViewProperty>
                                        {
                                            new ViewProperty { Name = "Action", Value = "CartSubtotalPercentOffAction" },
                                            version
                                        };
            var selectAction = Proxy.DoCommand(ShopsContainer.DoAction(selectView));
            selectAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            var addView = selectAction.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(selectView.Name));
            addView.Should().NotBeNull();
            addView?.Policies.Should().BeEmpty();
            addView?.Properties.Should().NotBeEmpty();
            addView?.ChildViews.Should().BeEmpty();
            addView?.Action.Should().Be("AddBenefit");
            version = addView?.Properties.FirstOrDefault(p => p.Name.Equals("Version"));
            addView.Properties = new ObservableCollection<ViewProperty>
                                     {
                                         new ViewProperty { Name = "Action", Value = "CartSubtotalPercentOffAction" },
                                         new ViewProperty { Name = "PercentOff", Value = "50" },
                                         version
                                     };
            var addAction = Proxy.DoCommand(ShopsContainer.DoAction(addView));
            addAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            var benefitId = addAction.Models.OfType<BenefitAdded>().FirstOrDefault()?.BenefitId;
            benefitId.Should().NotBeNullOrEmpty();

            return benefitId;
        }

        private static void EditBenefit(string promotionId, string benefitId)
        {
            Console.WriteLine("Begin EditBenefitUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "BenefitDetails", "EditBenefit", benefitId));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view?.ChildViews.Should().BeEmpty();
            view?.Action.Should().Be("EditBenefit");

            var version = view?.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Action", Value = "CartSubtotalPercentOffAction" },
                                      new ViewProperty { Name = "PercentOff", Value = "20" },
                                      version
                                  };
            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void DeleteBenefit(string promotionId, string benefitId)
        {
            Console.WriteLine("Begin DeleteBenefitUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "BenefitDetails", "DeleteBenefit", benefitId));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion

        #region Items

        private static void Items(string promotionId)
        {
            Console.WriteLine("Begin Items View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "Items", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
        }

        private static void ItemDetails(string promotionId, string itemId)
        {
            Console.WriteLine("Begin ItemDetails View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "ItemDetails", string.Empty, itemId));
            result.Should().NotBeNull();
            result.Policies.Should().BeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static void AddItem(string promotionId, string itemId, string catalog)
        {
            Console.WriteLine("Begin AddItemUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "ItemDetails", "AddItem", string.Empty));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view?.ChildViews.Should().BeEmpty();
            view?.Action.Should().Be("AddItem");

            var version = view?.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Catalog", Value = catalog },
                                      new ViewProperty { Name = "ItemId", Value = itemId },
                                      new ViewProperty { Name = "Excluded", Value = "false" },
                                      version
                                  };
            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            action.Models.OfType<PromotionItemAdded>().FirstOrDefault()?.PromotionItemId.Should().NotBeNullOrEmpty();
        }

        private static void RemoveItem(string promotionId, string itemId)
        {
            Console.WriteLine("Begin RemoveItemUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(promotionId, "ItemDetails", "RemoveItem", itemId));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

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
