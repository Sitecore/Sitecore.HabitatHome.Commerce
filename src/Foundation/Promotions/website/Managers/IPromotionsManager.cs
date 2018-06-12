using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.HabitatHome.Foundation.Promotions.Models;

namespace Sitecore.HabitatHome.Foundation.Promotions.Managers
{
    public interface IPromotionsManager
    {
        IEnumerable<Promotion> GetActivePromotions(string productId);
        IEnumerable<Promotion> GetAllPromotions();

    }
}