using System;                         
using System.Linq;
using System.Linq.Expressions;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.XA.Foundation.Search.Models;
using Sitecore.XA.Foundation.Search.Services;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.XA.Foundation.Search;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    public class SearchService : XA.Foundation.Search.Services.SearchService, ISearchService
    {                  
        public new IQueryable<ContentPage> GetQuery(string query, string scope, string language, Coordinates center, string site, string itemId, out string indexName)
        {
            language = Sitecore.Context.Language.Name;
            return base.GetQuery(query, scope, language, center, site, itemId, out indexName);
        }

        protected override Expression<Func<ContentPage, bool>> PageOrMediaPredicate(string siteName)
        {
            Item homeItem = this.SearchContextService.GetHomeItem(siteName);
            if (homeItem == null)
            {
                return PredicateBuilder.False<ContentPage>();
            }                                                      

            Expression<Func<ContentPage, bool>> expression = i => i.IsSearchable;
            Item settingsItem = MultisiteContext.GetSettingsItem(homeItem);
            if (settingsItem != null)
            {
                MultilistField associatedContentSetting = settingsItem.Fields[Templates._SearchCriteria.Fields.AssociatedContent];
                if (associatedContentSetting != null)
                {
                    if (associatedContentSetting.TargetIDs.Any())
                    {
                        foreach(ID id in associatedContentSetting.TargetIDs) { 
                                expression = expression.Or(i => i.RawPath == id.ToSearchID() && i.IsSearchable);     
                        }
                    }
                }
                MultilistField associatedMediaSetting = settingsItem.Fields[Templates._SearchCriteria.Fields.AssociatedMedia];
                if (associatedMediaSetting != null)
                {
                    if (associatedMediaSetting.TargetIDs.Any())
                    {
                        foreach (Item item in associatedMediaSetting.GetItems())
                        {
                            expression = expression.Or(i => i.RawPath == item.ID.ToSearchID());
                        }
                    }        
                }                                            
            }
            return expression;
        }                                                          
    }
}