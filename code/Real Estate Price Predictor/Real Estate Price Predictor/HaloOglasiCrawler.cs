using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using Real_Estate_Price_Predictor.Repos;

namespace Real_Estate_Price_Predictor
{
    class HaloOglasiCrawler: IDisposable
    {
        public HashSet<string> VisitedPages { get; } = new HashSet<string>();
        public List<RealEstate> RealEstates { get; } = new List<RealEstate>();
        public ChromeDriver Driver { get; set; }
        public ChromeOptions ChromeOptions { get; set; }

        public int Id = ++idGen;

        private static int idGen = 0;

        public HaloOglasiCrawler()
        {
            SetupProxy();
        }

        public async Task StartCrawling()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            Console.WriteLine($"[T{Id}] Started!");
            for (int i = 0; i < 3; ++i)
            {
                var realEstate = await ParseRealEstate("https://www.halooglasi.com/nekretnine/izdavanje-stanova/centar---beograd-na-vodi-bw-id35120/5425635493498?kid=4");
                Console.WriteLine($"[T{Id}] {ChromeOptions.Proxy.HttpProxy} -> PARSED {realEstate.Url}  - {s.Elapsed}");
            }
            Console.WriteLine($"[T{Id}] DONE - {s.Elapsed}");
        }

        private void SetupProxy()
        {
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            ChromeOptions = new ChromeOptions
            {
                Proxy = new Proxy
                {
                    HttpProxy = ProxyRepo.GetProxy()
                }
            };
            ChromeOptions.AddArguments("disable-gpu", "silent", "headless", "log-level=3");
            Driver = new ChromeDriver(service, ChromeOptions);
        }

        private void UseNewProxy()
        {
            ChromeOptions.Proxy = new Proxy
            {
                HttpProxy = ProxyRepo.GetProxy()
            };
        }

        private Task<RealEstate> ParseRealEstate(string url)
        {
            return Task.Run(() =>
            {
                Driver.Navigate().GoToUrl(url);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(Driver.PageSource);

                var scraper = new HaloOglasiScraper();

                var realEstate = scraper.ScrapeRealEstate(htmlDocument);

                realEstate.Url = url;

                return realEstate;

            });
        }

        public void Dispose()
        {
            Driver.Close();
        }
    }
}
