using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstatePricePredictor
{
    public class Preprocessor
    {
        public List<Instance> Train;
        public List<Instance> Test;
        public Preprocessor(double testSize)
        {
            Console.WriteLine("Preprocessing started");
            using (var db = new RealEstateContext())
            {
                var flatsForSale = db.RealEstates.Where(re => re.City == "Beograd" && re.HousingType == 0 && re.OfferType == 0);
                int totalCount = flatsForSale.Count();
                int splitIndex = Convert.ToInt32(Math.Floor(totalCount * (1 - testSize)));

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
                        Data = new double[]
                        {
                            e.Price,
                            e.Distance,
                            e.Quadrature,
                            Convert.ToDouble(e.Registered),
                            Convert.ToDouble(e.Floor),
                            Convert.ToDouble(e.RoomCount),
                        }
                    });

                var instancesCount = instances.Count();
                Train = instances.Take(splitIndex).ToList();
                Test = instances.Skip(splitIndex).ToList();
            }
            Console.WriteLine("Preprocessing finished");
        }
    }
}
