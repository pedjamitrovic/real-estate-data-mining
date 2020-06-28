using System;
using System.Threading.Tasks;
using System.Device.Location;

namespace RealEstatePricePredictor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Phase 1
            // ProxyRepo.Instance.Init();
            // CrawlAsync().Wait();
            // ScrapeAsync().Wait();

            // Phase 4
            new LinearRegression(100, 100, 100, 0.2);
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
    }
}
