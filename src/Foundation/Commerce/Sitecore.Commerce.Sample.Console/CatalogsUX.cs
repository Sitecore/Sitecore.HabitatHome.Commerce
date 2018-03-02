namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using FluentAssertions;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Extensions;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class CatalogsUX
    {
        private const string CatalogName = 
            "ConsoleCatalog";

        private static readonly string CatalogId = 
            $"Entity-Catalog-{CatalogName}";

        private static Sitecore.Commerce.Engine.Container ShopsContainer =
            new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin CatalogsUX");

            AddCatalog();
            EditCatalog();

            watch.Stop();

            Console.WriteLine($"End CatalogsUX:{watch.ElapsedMilliseconds} ms");
        }

        private static void AddCatalog()
        {
            Console.WriteLine("Begin AddCatalog");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(string.Empty, "Details", "AddCatalog", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            view.Properties = new ObservableCollection<ViewProperty>
            {
                new ViewProperty {Name = "Name", Value = $"{CatalogName}$%^*&{{"},
            };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            view.Properties = new ObservableCollection<ViewProperty>
            {
                new ViewProperty {Name = "Name", Value = CatalogName},
                new ViewProperty {Name = "DisplayName", Value = "Console UX Catalog"}
            };

            result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void EditCatalog()
        {
            Console.WriteLine("Begin EditCatalog");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CatalogId, "Details", "EditCatalog", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
            {
                new ViewProperty {Name = "DisplayName", Value = "Console UX Catalog (updated)"},
                version
            };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }
    }
}
