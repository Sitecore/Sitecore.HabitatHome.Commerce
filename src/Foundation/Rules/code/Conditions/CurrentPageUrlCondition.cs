using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System.Text.RegularExpressions;
using System.Web;

namespace Sitecore.Foundation.Rules.Conditions
{
    public class CurrentPageUrlCondition<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string CategoryId { get; set; }
        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            
            if (ID.TryParse(this.CategoryId, out var category))
            {
                Item categoryItem = Database.GetDatabase("master").GetItem(category);
                string categoryName = categoryItem.DisplayName.ToLower();
                var categoryMatch = Regex.Match(categoryName, "\\b[^\\d\\W]+\\b", RegexOptions.CultureInvariant);

                if (categoryMatch.Success)
                {
                    if (CheckContains(categoryMatch, HttpContext.Current.Request.Url.AbsolutePath.ToLower()))
                    {
                        Log.Info("Promotion persinalized for category " + categoryName, this);
                        return true;
                    }
                    Log.Info("Promotion NOT persinalized for category " + categoryName, this);
                }

            }

            Log.Info("Category personalization failed for " + CategoryId, this);
            return false;
        }

        private bool CheckContains(Match category, string url)
        {
            if (!url.Contains(category.Value))
            {
                return false;
            }

            category = category.NextMatch();
            if (category.Success)
            {
                CheckContains(category, url);
            }

            return true;
        }
    }
}