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
            // CrawlAsync().Wait();
            ScrapeAsync().Wait();
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
