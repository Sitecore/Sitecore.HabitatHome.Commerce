using Sitecore.Commerce.Engine.Connect.DataProvider.Extensions;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace Sitecore.Foundation.Catalog.Pipelines
{
    public class CatalogLinkProvider : Commerce.XA.Foundation.Catalog.Pipelines.CatalogLinkProvider 
    {
        public override string GetItemUrl(Item item, UrlOptions options)
        {
            if (item.IsCatalogItem())
            {
                return GetDynamicUrl(item, new LinkUrlOptions
                {
                    Language = options.Language, Site = options.Site.Name

                });
            }

            return base.GetItemUrl(item, options);
        }
    }
}