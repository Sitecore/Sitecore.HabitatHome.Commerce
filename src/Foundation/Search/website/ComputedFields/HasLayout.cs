using Sitecore.Commerce.Engine.Connect.DataProvider.Extensions;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;

namespace Sitecore.HabitatHome.Foundation.Search.ComputedFields
{
    public class HasLayout : XA.Foundation.Search.ComputedFields.HasLayout
    {                                      
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
            if (item == null)
            {
                return false;
            }

            if (item.IsCatalogItem())
            {
                return true;
            }

            return base.ComputeFieldValue(indexable);
        }
    }
}