using System.Linq;
using Sitecore.Commerce.Engine.Connect.DataProvider.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;

namespace Sitecore.Foundation.Search.ComputedFields
{
    public class ProductTags : AbstractComputedIndexField
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
                string tags = item["Tags"];
                if (!string.IsNullOrEmpty(tags))
                {
                    return tags.Split('|');
                }
            }

            return Enumerable.Empty<string>();
        }
    }
}