using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System.Text.RegularExpressions;
using System.Web;

namespace Sitecore.Foundation.Rules.Conditions
{
    public class CurrentPageUrlCondition<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string CategoryPath { get; set; }
        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            string category = CategoryPath.Substring(CategoryPath.LastIndexOf("-") + 1).ToLower();
            if (!string.IsNullOrWhiteSpace(category))
            {
                var categoryMatch = Regex.Match(category, "\\b[^\\d\\W]+\\b", RegexOptions.CultureInvariant);

                if (categoryMatch.Success)
                {
                    if (CheckContains(categoryMatch, HttpContext.Current.Request.Url.AbsolutePath.ToLower()))
                    {
                        return true;
                    }
                }
            }
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