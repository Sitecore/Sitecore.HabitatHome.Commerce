using Sitecore.Commerce.Engine.Connect.DataProvider.Extensions;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;

namespace Sitecore.Foundation.Search.ComputedFields
{
    public class AggregatedContent : XA.Foundation.Search.ComputedFields.AggregatedContent
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;
            if (item == null)
            {
                return null;
            }

            if (item.IsCatalogItem())
            {
                //add product content  
                ProviderIndexConfiguration config = ContentSearchManager.GetIndex(indexable).Configuration;
                return ExtractTextFields(item, config);
            }

            return base.ComputeFieldValue(indexable);
        }

    }
}