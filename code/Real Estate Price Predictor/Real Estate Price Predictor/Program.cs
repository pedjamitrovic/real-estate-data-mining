using Real_Estate_Price_Predictor.Repos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Real_Estate_Price_Predictor
{
    class Program
    {
        static void Main(string[] args)
        {
            ProxyRepo.Instance.Init();
            ScrapeAsync();
            Console.ReadLine();
        }

        public static async void CrawlAsync()
        {
            using (var crawler = new HaloOglasiCrawler())
            {
                await crawler.CrawlRealEstatePages();
            }
        }

        public static async void ScrapeAsync()
        {
            using (var crawler = new HaloOglasiCrawler())
            {
                await crawler.ParseRealEstates();
            }
        }
    }
}
