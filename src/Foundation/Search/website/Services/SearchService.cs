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
using System.Collections.Generic;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    public class SearchService : XA.Foundation.Search.Services.SearchService, ISearchService
    {                  
        public new IQueryable<ContentPage> GetQuery(SearchQueryModel searchQueryModel, out string indexName)
        {
            //todo: determine if this override is still necessary in SXA 1.8
            searchQueryModel.Languages = new List<string> { Sitecore.Context.Language.Name };
            return base.GetQuery(searchQueryModel, out indexName);
        }

        protected override Expression<Func<ContentPage, bool>> PageOrMediaPredicate(string siteName)
        {
            // this override is necessary to ensure catalog items are included in search results since they don't live under the site's home node
            // todo: determine if this override is still necessary in CXA 9.1 (the 9.0.2 catalog location update probably made this obsolete)
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