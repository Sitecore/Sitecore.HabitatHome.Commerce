using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System.Runtime.CompilerServices;
using Sitecore.Commerce.Plugin.Inventory;
using Plugin.Demo.HabitatHome.StoreInventorySet.Components;
using Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Arguments;
using System.Device.Location;
using Plugin.Demo.HabitatHome.StoreInventorySet.Entities;

namespace Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Blocks
{
    [PipelineDisplayName("StoreInventory.block.GetStoreDetails")]
    public class GetNearestStoreDetailsByLocationBlock : PipelineBlock<GetNearestStoreDetailsByLocationArgument, List<NearestStoreLocation>, CommercePipelineExecutionContext>
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;

        public GetNearestStoreDetailsByLocationBlock(IFindEntitiesInListPipeline findEntitiesInListPipeline)
      : base((string)null)
        {
            this._findEntitiesInListPipeline = findEntitiesInListPipeline;
        }

        public override async Task<List<NearestStoreLocation>> Run(GetNearestStoreDetailsByLocationArgument locationInfo, CommercePipelineExecutionContext context)
        {
            List<InventorySet> inventorySets = new List<InventorySet>();

            GetNearestStoreDetailsByLocationBlock getNearestStoreDetailsByLocationBlock = this;

            FindEntitiesInListArgument entitiesInListArgument = await getNearestStoreDetailsByLocationBlock._findEntitiesInListPipeline.Run(new FindEntitiesInListArgument(typeof(InventorySet), string.Format("{0}", (object)CommerceEntity.ListName<InventorySet>()), 0, int.MaxValue), context);

            if (entitiesInListArgument != null)
            {
                CommerceList<CommerceEntity> list = entitiesInListArgument.List;
                if (list != null)
                    list.Items.ForEach((Action<CommerceEntity>)(item =>
                    {
                        InventorySet inventorySet = (InventorySet)item;
                        

                        inventorySets.Add(inventorySet);
                    }));
            }

            var storeComponents = inventorySets.Select(x => x.GetComponent<StoreDetailsComponent>());

            storeComponents = storeComponents.Where(x => x.Lat != null).ToList();

            List<Locations> locations = new List<Locations>();
            locations.AddRange(storeComponents.Select(x => x != null ? new Locations() { City = x.City, Latitude = Convert.ToDouble(x.Lat), Longitude = Convert.ToDouble(x.Long) } : new Locations()));

            var coord = new GeoCoordinate(locationInfo.Latitude, locationInfo.Longitude);


            var nearestStoresinOrder = locations.Select(x => new GeoCoordinate(x.Latitude, x.Longitude))
                                   .OrderBy(x => x.GetDistanceTo(coord)).Select(z => new Locations { Distance = z.GetDistanceTo(coord), Latitude = z.Latitude, Longitude = z.Longitude }).ToList();


            List<NearestStoreLocation> stores = new List<NearestStoreLocation>();


            stores.AddRange(nearestStoresinOrder.Select(x => new NearestStoreLocation() { Distance = x.Distance, InventoryStoreId = GetStoreId(x.Latitude, x.Longitude, inventorySets),
                Address = GetStoreDetails(x.Latitude, x.Longitude, inventorySets).GetComponent<StoreDetailsComponent>().Address,
                Longitude = x.Longitude,
                Latitude = x.Latitude, Name = GetStoreDetails(x.Latitude, x.Longitude, inventorySets).GetComponent<StoreDetailsComponent>().Name ,
                City = GetStoreDetails(x.Latitude, x.Longitude, inventorySets).GetComponent<StoreDetailsComponent>().City , Zip = GetStoreDetails(x.Latitude, x.Longitude, inventorySets).GetComponent<StoreDetailsComponent>().ZipCode
            }));

            return stores;
        }

        private InventorySet GetStoreDetails(double latitude, double longitude, List<InventorySet> inventorySets)
        {
            return inventorySets.Where(x => x.GetComponent<StoreDetailsComponent>().Lat == Convert.ToString(latitude)).FirstOrDefault();            
        }

        private string GetStoreId(double latitude, double longitude, List<InventorySet> inventorySets)
        {
            return inventorySets.Where(x => x.GetComponent<StoreDetailsComponent>().Lat == Convert.ToString(latitude)).FirstOrDefault().FriendlyId;
        }
    }

    public class Locations
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string City { get; set; }
        public double Distance { get; set; }
    }
}
