using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Device.Location;
using System.IO;

namespace RealEstatePricePredictor
{
    public class NeighborhoodDistance
    {
        [Key] public int ID { get; set; }
        public string Neighborhood { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double Distance { get; set; }

        public static void ReadCsvAndInsertData()
        {
            string FilePath = @"neighborhoods.csv";
            IEnumerable<string> rows;
            using (FileStream fs = File.OpenRead(FilePath))
            {
                rows = File.ReadLines(FilePath).Skip(1);
            }

            using (var db = new RealEstateContext())
            {
                // City center
                var sCoord = new GeoCoordinate(44.816088, 20.459962);
                foreach (string row in rows)
                {
                    var values = row.Split(',');
                    var nd = new NeighborhoodDistance
                    {
                        Neighborhood = values[0],
                        Lat = double.Parse(values[1]),
                        Lng = double.Parse(values[2])
                    };
                    var eCoord = new GeoCoordinate(nd.Lat, nd.Lng);
                    nd.Distance = sCoord.GetDistanceTo(eCoord) / 1000;
                    db.NeighborhoodDistances.Add(nd);
                }
                db.SaveChanges();
            }
        }
    }
}
