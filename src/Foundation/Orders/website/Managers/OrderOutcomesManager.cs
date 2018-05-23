using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.CustomModels.Outcomes;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;

namespace Sitecore.HabitatHome.Foundation.Orders.Managers
{
    public class OrderOutcomesManager : IOrderOutcomesManager
    {
        public IEnumerable<SubmittedOrderOutcome> GetSubmittedOrderOutcomes(double? pastDaysAmount)
        {
            var submittedOrderOutcomes = new List<SubmittedOrderOutcome>();
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                var fullUserName = Sitecore.Context.GetUserName();
                var domainStrip = fullUserName.Split('\\');
                var userName = domainStrip.Count() > 1 ? domainStrip[1] : domainStrip[0];
                var contactReference = new IdentifiedContactReference("username", userName);
                var contactFacets = client.Model.Facets.Where(c => c.Target == EntityType.Contact).Select(x => x.Name);
                var interactionFacets = client.Model.Facets.Where(c => c.Target == EntityType.Interaction).Select(x => x.Name);
                var contact = client.Get<Contact>(contactReference, new ContactExpandOptions(contactFacets.ToArray())
                {
                    Interactions = new RelatedInteractionsExpandOptions(interactionFacets.ToArray())
                    {
                        EndDateTime = DateTime.MaxValue,
                        StartDateTime = DateTime.MinValue
                    }
                });
                if (contact != null)
                {
                    foreach (var interaction in contact.Interactions)
                    {
                        var submittedOrderEvents = interaction.Events.OfType<SubmittedOrderOutcome>().OrderBy(ev => ev.Timestamp).ToList();
                        if (pastDaysAmount.HasValue)
                            submittedOrderEvents = submittedOrderEvents.Where(ev => ev.Timestamp > DateTime.Now.AddDays(-(pastDaysAmount.Value))).ToList();
                        submittedOrderOutcomes.AddRange(submittedOrderEvents);
                    }
                }
            }
            return submittedOrderOutcomes;
        }
    }
}