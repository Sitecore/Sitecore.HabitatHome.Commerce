using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Data.Items;
using Sitecore.XA.Feature.Navigation.Models;           
using Sitecore.XA.Foundation.Mvc.Repositories.Base;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.XA.Foundation.Common;

namespace Sitecore.HabitatHome.Feature.Catalog.Repositories
{
    public class BreadcrumbRepository : Sitecore.XA.Feature.Navigation.Repositories.Breadcrumb.BreadcrumbRepository
    {

        private readonly IItemTypeProvider _itemTypeProvider;
        private readonly ISiteContext _siteContext;
        private readonly IStorefrontContext _storefrontContext;

        public BreadcrumbRepository(IItemTypeProvider itemTypeProvider, ISiteContext siteContext, IStorefrontContext storefrontContext) : base()
        {
            _itemTypeProvider = itemTypeProvider;
            _siteContext = siteContext;
            _storefrontContext = storefrontContext;
        }                             

        public override IEnumerable<BreadcrumbRenderingModel> GetBreadcrumbItems(Item currentItem, Item rootItem)
        {
            var breadcrumbItems = BuildBreadcrumb(currentItem, rootItem).Where(ShowInBreadcrumb).ToList();
            int count = breadcrumbItems.Count;

            // Null is passed in for the Children parameter as it doesn't seem relevant for commerce catalog
            return breadcrumbItems.Select((item, index) => CreateBreadcrumbModel(item, index, count, null, null));
        }

        protected override BreadcrumbRenderingModel CreateBreadcrumbModel(Item item, int index, int count, IEnumerable<BreadcrumbRenderingModel> children, string Name)
        {
            var cssClasses = new List<string>();        
            if (index == 0)
            {
                cssClasses.Add("home");
            }

            return new BreadcrumbRenderingModel(item)
            {
                Name = Name,
                Children = children,
                CssClasses = cssClasses,
                VariantFields = VariantFields,
                Separator = Rendering.Parameters[Sitecore.XA.Feature.Navigation.Constants.Separator]
            };
        }

        protected override bool ShowInBreadcrumb(Item item)
        {
            var itemType = _itemTypeProvider.GetItemType(item);
            if (itemType == Sitecore.Commerce.XA.Foundation.Common.Constants.ItemTypes.Category ||
                itemType == Sitecore.Commerce.XA.Foundation.Common.Constants.ItemTypes.Product)
            {
                return true;
            }

            return base.ShowInBreadcrumb(item);
        }    

        public override IRenderingModelBase GetModel()
        {                         
            IEnumerable<BreadcrumbRenderingModel> breadcrumb;

            Item rootItem = Context.Database.GetItem(Rendering.Parameters[Sitecore.XA.Feature.Navigation.Constants.BreadcrumbRoot] ?? string.Empty);
            
            Item commerceRootItem = Context.Database.GetItem(_storefrontContext.CurrentStorefront.GetStartNavigationCategory());
            if (_siteContext.IsCategory || _siteContext.IsProduct)
            {
                // It's a catalog item so we need to concatenate the ancestors for the catalog with the ancestors on the site itself                                
                breadcrumb = GetBreadcrumbItems(CurrentItem, rootItem)
                    .Concat(GetBreadcrumbItems(_siteContext.CurrentCatalogItem, commerceRootItem))
                    .Where(a => a.Item.ID != commerceRootItem.ID);              
            }
            else
            {                                                
                breadcrumb = GetBreadcrumbItems(CurrentItem, rootItem);
            }

            BreadcrumbRenderingModel model = new BreadcrumbRenderingModel
            {
                Separator = Rendering.Parameters[Sitecore.XA.Feature.Navigation.Constants.Separator]
            };

            FillBaseProperties(model);

            var breadcrumbModels = breadcrumb as BreadcrumbRenderingModel[] ?? breadcrumb.ToArray();
            if (!breadcrumbModels.Any())
            {
                var fakeNav = Enumerable.Empty<BreadcrumbRenderingModel>();
                if (Context.PageMode.IsExperienceEditor)
                {
                    fakeNav = new FakeBreadcrumbRepository().GetBreadcrumbItems(null, null);
                }
                model.IsFake = true;
                model.Children = fakeNav;
            }
            else
            {
                model.Children = breadcrumbModels;
            }

            return model;
        }
    }
}