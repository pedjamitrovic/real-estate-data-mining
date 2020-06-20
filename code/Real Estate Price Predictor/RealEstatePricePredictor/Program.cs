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
            ScrapeAsync().Wait();
            Console.ReadLine();
            //var query = from estate in db.RealEstates select re.City;
            //query.ToList();
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
    }
}
