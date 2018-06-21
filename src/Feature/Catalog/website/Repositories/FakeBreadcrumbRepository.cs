using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.XA.Feature.Navigation.Models;

namespace Sitecore.HabitatHome.Feature.Catalog.Repositories
{
    public class FakeBreadcrumbRepository : Sitecore.XA.Feature.Navigation.Repositories.Breadcrumb.BreadcrumbRepository
    {
        public override IEnumerable<BreadcrumbRenderingModel> GetBreadcrumbItems(Item currentItem, Item rootItem)
        {
            var fakeBreadcrumb = new List<BreadcrumbRenderingModel>();
            for (int i = 0; i < 3; i++)
            {
                BreadcrumbRenderingModel fakeModel = CreateBreadcrumbModel(null, i, 2, null, $"Tag Level {i}");
                fakeBreadcrumb.Add(fakeModel);
            }

            return fakeBreadcrumb;
        }
    }
}