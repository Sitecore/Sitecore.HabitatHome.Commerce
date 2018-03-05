namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using FluentAssertions;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class CategoriesUX
    {
        private const string CatalogName =
            "Adventure Works Catalog";

        private const string CategoryName =
            "ConsoleCategory";

        private static readonly string CategoryId =
            $"Entity-Category-{CatalogName}-{CategoryName}";

        private static readonly Sitecore.Commerce.Engine.Container ShopsContainer = 
            new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin CategoriesUX");

            AddCategory();
            EditCategory();

            watch.Stop();

            Console.WriteLine($"End CategoriesUX :{watch.ElapsedMilliseconds} ms");
        }

        private static void AddCategory()
        {
            Console.WriteLine("Begin AddCategory");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView($"Entity-Catalog-{CatalogName}", "Details", "AddCategory", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
            {
                new ViewProperty {Name = "Name", Value = CategoryName},
                new ViewProperty {Name = "DisplayName", Value = "Console UX Category"},
                new ViewProperty {Name = "Description", Value = "Console UX Category Description"},
                version
            };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void EditCategory()
        {
            Console.WriteLine("Begin EditCategory");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(CategoryId, "Details", "EditCategory", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
            {
                new ViewProperty {Name = "DisplayName", Value = "Console UX Category (updated)"},
                new ViewProperty {Name = "Description", Value = "Console UX Category Description"},
                version
            };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }
    }
}
