using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstatePricePredictor
{
    public class Preprocessor
    {
        public List<Instance> train;
        public List<Instance> test;
        public Preprocessor(double testSize)
        {
            using (var db = new RealEstateContext())
            {
                var flatsForSale = db.RealEstates.Where(re => re.City == "Beograd" && re.HousingType == 0 && re.OfferType == 0);
                int totalCount = flatsForSale.Count();
                int splitIndex = Convert.ToInt32(Math.Floor(totalCount * testSize));

                var instances = flatsForSale
                    .Join(
                    db.NeighborhoodDistances,
                    re => re.Neighborhood,
                    nd => nd.Neighborhood,
                    (re, nd) => new { nd.Distance, re.Quadrature, re.Registered, re.Floor, re.RoomCount, re.Price }
                    )
                    .AsEnumerable()
                    .Select(e => new Instance
                    {
                        Distance = e.Distance,
                        Quadrature = e.Quadrature,
                        Registered = Convert.ToDouble(e.Registered),
                        Floor = Convert.ToDouble(e.Floor),
                        RoomCount = Convert.ToDouble(e.RoomCount),

                        Price = e.Price
                    });
                var instancesCount = instances.Count();
                train = instances.Take(totalCount - splitIndex).ToList();
                test = instances.Skip(totalCount - splitIndex).ToList();
            }
        }
    }
}
