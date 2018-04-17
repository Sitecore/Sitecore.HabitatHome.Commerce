using Sitecore.Commerce.CustomModels.Outcomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Foundation.Commerce.OrderOutcomes.Managers
{ 
    public interface IOrderOutcomesManager
    {
        IEnumerable<SubmittedOrderOutcome> GetSubmittedOrderOutcomes(double? pastDaysAmount);
    }
}