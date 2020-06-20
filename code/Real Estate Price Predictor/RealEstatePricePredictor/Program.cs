using System;
using System.Threading.Tasks;
using System.Linq;

namespace RealEstatePricePredictor
{
    class Program
    {
        static void Main(string[] args)
        {
            ProxyRepo.Instance.Init();
            //ScrapeAsync().Wait();
            Damy();
            Console.ReadLine();
        }

        public static async Task CrawlAsync()
        {
            using (var crawler = new HaloOglasiCrawler())
            {
                await crawler.CrawlRealEstatePages();
            }
        }

        public static async Task ScrapeAsync()
        {
            using (var crawler = new HaloOglasiCrawler())
            {
                await crawler.ParseRealEstates();
            }
        }

        public static void Damy()
        {
            RealEstate re = new RealEstate
            {
                Neighborhood = "zarkovo"
            };
            using (var db = new RealEstateContext())
            {
                //db.RealEstates.Add(re);
                //db.SaveChanges();
                //var query = from estate in db.RealEstates select re.City;
                //query.ToList();
            }
        }
    }
}
