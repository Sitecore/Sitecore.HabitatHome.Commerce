using Sitecore.Commerce.Engine.Connect.DataProvider.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;

namespace Sitecore.HabitatHome.Foundation.Search.ComputedFields
{
    public class ProductBrand   : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
            if (item == null)
            {
                return string.Empty;
            }

            if (item.IsCatalogItem())
            {
                return item["Brand"];
            }

            return string.Empty;
        }
    }
}