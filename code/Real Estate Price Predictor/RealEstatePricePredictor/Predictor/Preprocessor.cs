using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstatePricePredictor
{
    public class Preprocessor
    {
        public List<Instance> Train;
        public List<Instance> Test;
        public double[] columnMin;
        public double[] columnMax;
        public Preprocessor(double testSize)
        {
            Console.WriteLine("Preprocessing started");
            using (var db = new RealEstateContext())
            {
                var flatsForSale = db.RealEstates.Where(re => re.City == "Beograd" && re.HousingType == 0 && re.OfferType == 0);
                int splitIndex = Convert.ToInt32(Math.Floor(flatsForSale.Count() * (1 - testSize)));

                var instances = flatsForSale
                    .Join(
                    db.NeighborhoodDistances,
                    re => re.Neighborhood,
                    nd => nd.Neighborhood,
                    (re, nd) => new { nd.Distance, re.Quadrature, re.Floor, re.RoomCount, re.Price }
                    )
                    .AsEnumerable()
                    .Select(e => new Instance
                    {
                        Data = new double[]
                        {
                            Math.Log(e.Price) + 1,
                            e.Distance,
                            e.Quadrature,
                            Convert.ToDouble(e.Floor),
                            Convert.ToDouble(e.RoomCount),
                        },
                        OriginalPrice = e.Price
                    }).ToList();

                instances.Shuffle();

                Train = instances.Take(splitIndex).ToList();
                Test = instances.Skip(splitIndex).ToList();

                CalculateNormalizationValues(Train);
                Normalize(Train);
                Normalize(Test);
            }
            Console.WriteLine("Preprocessing finished");
        }

        private void CalculateNormalizationValues(List<Instance> instances)
        {
            int columnCount = instances[0].Data.Count();

            columnMin = new double[columnCount];
            for (int i = 0; i < columnCount; ++i)
            {
                columnMin[i] = double.PositiveInfinity;
            }

            columnMax = new double[columnCount];
            for (int i = 0; i < columnCount; ++i)
            {
                columnMax[i] = double.NegativeInfinity;
            }

            foreach (var instance in instances)
            {
                for (int i = 0; i < columnCount; ++i)
                {
                    var value = instance.Data[i];
                    if (value < columnMin[i])
                    {
                        columnMin[i] = value;
                    }
                    if (value > columnMax[i])
                    {
                        columnMax[i] = value;
                    }
                }
            }
        }

        private void Normalize(List<Instance> instances)
        {
            foreach (var instance in instances)
            {
                Normalize(instance);
            }
        }

        public void Normalize(Instance instance)
        {
            int columnCount = instance.Data.Count();

            for (int i = 0; i < columnCount; ++i)
            {
                instance.Data[i] = (instance.Data[i] - columnMin[i]) / (columnMax[i] - columnMin[i]);
            }
        }

        public void Denormalize(Instance instance)
        {
            int columnCount = instance.Data.Count();

            for (int i = 0; i < columnCount; ++i)
            {
                instance.Data[i] = columnMin[i] + instance.Data[i] * (columnMax[i] - columnMin[i]);
            }
        }

        public double DenormalizedPrice(double normalizedPrice)
        {
            return columnMin[0] + normalizedPrice * (columnMax[0] - columnMin[0]);
        }

        
    }

    public static class ShuffleExtension
    {
        public static void Shuffle<T>(this List<T> list)
        {
            Random random = new Random();
            int n = list.Count();

            for (int i = n - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                T value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }
        }
    }
}
