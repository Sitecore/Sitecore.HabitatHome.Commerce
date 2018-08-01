using System.Collections.Generic;
using Sitecore.Commerce.CustomModels.Outcomes;  

namespace Sitecore.HabitatHome.Foundation.Orders.Managers
{ 
    public interface IOrderOutcomesManager
    {
        IEnumerable<SubmittedOrderOutcome> GetSubmittedOrderOutcomes(double? pastDaysAmount);
    }
}