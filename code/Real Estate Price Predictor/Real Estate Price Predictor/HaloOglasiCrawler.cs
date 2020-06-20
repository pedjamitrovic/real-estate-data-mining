using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using Real_Estate_Price_Predictor.Repos;
using Real_Estate_Price_Predictor.Model;
using System.IO;
using System.Text;

namespace Real_Estate_Price_Predictor
{
    class HaloOglasiCrawler : IDisposable
    {
        public List<string> RealEstatePages { get; } = new List<string>();
        public List<RealEstate> RealEstates { get; } = new List<RealEstate>();
        public ChromeDriver Driver { get; set; }
        public ChromeOptions ChromeOptions { get; set; }

        public int Id = ++idGen;

        private static int idGen = 0;
        private static List<HaloOglasiCrawlerOptions> HocOptions = new List<HaloOglasiCrawlerOptions>();
        private static readonly string RealEstateLinksFilePath = @"real-estate-links.txt";
        
        public HaloOglasiCrawler()
        {
            SetupProxy();
            SetupHocOptions();
        }

        public async Task CrawlRealEstatePages()
        {
            await Task.Run(async () =>
            {
                if (File.Exists(RealEstateLinksFilePath))
                {
                    File.Delete(RealEstateLinksFilePath);
                }
                using (FileStream fs = File.Create(RealEstateLinksFilePath))
                {
                    for (int i = 0; i < HocOptions.Count; ++i)
                    {
                        for (int j = 0; j < HocOptions[i].PageCount; ++j)
                        {
                            List<string> realEstatePages = await ParsePage($"{HocOptions[i].StartUrl}?page={j}");

                            foreach (string realEstatePage in realEstatePages)
                            {
                                byte[] info = new UTF8Encoding(true).GetBytes($"https://www.halooglasi.com{realEstatePage}{Environment.NewLine}");
                                fs.Write(info, 0, info.Length);
                            }

                            UseNewProxy();
                        }
                    }
                }
            });
        }

        public async Task ParseRealEstates()
        {
            await Task.Run(async () =>
            {
                var re = await ParseRealEstate("https://www.halooglasi.com/nekretnine/prodaja-kuca/sd-luksuzna-vila/5425635644763?kid=4&sid=1592140358977");
                Console.WriteLine(re.ToString());
                
                //ReadRealEstateLinks();
                //for (int i = 0; i< 10; ++i)
                //{
                //    var re = await ParseRealEstate(RealEstatePages[i]);
                //    Console.WriteLine(re.ToString());
                //}
                //for (int i = 15880; i < 15890; ++i)
                //{
                //    var re = await ParseRealEstate(RealEstatePages[i]);
                //    Console.WriteLine(re.ToString());
                //}
                //for (int i = 21330; i < 21340; ++i)
                //{
                //    var re = await ParseRealEstate(RealEstatePages[i]);
                //    Console.WriteLine(re.ToString());
                //}
                //for (int i = 22222; i < 22232; ++i)
                //{
                //    var re = await ParseRealEstate(RealEstatePages[i]);
                //    Console.WriteLine(re.ToString());
                //}
            });
        }


        public void Dispose()
        {
            Driver.Close();
        }

        private void SetupProxy()
        {
            var service = ChromeDriverService.CreateDefaultService(@"D:\Master\PSZ\real-estate-data-mining\code\Real Estate Price Predictor\Real Estate Price Predictor\bin\Debug\netcoreapp2.0");
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

        private void SetupHocOptions()
        {
            HocOptions.Add(new HaloOglasiCrawlerOptions { StartUrl = "https://www.halooglasi.com/nekretnine/prodaja-stanova", PageCount = 650 });
            HocOptions.Add(new HaloOglasiCrawlerOptions { StartUrl = "https://www.halooglasi.com/nekretnine/izdavanje-stanova", PageCount = 250 });
            HocOptions.Add(new HaloOglasiCrawlerOptions { StartUrl = "https://www.halooglasi.com/nekretnine/prodaja-kuca", PageCount = 200 });
            HocOptions.Add(new HaloOglasiCrawlerOptions { StartUrl = "https://www.halooglasi.com/nekretnine/izdavanje-kuca", PageCount = 15 });
        }

        private void UseNewProxy()
        {
            ChromeOptions.Proxy = new Proxy
            {
                HttpProxy = ProxyRepo.GetProxy()
            };
        }

        private Task<List<string>> ParsePage(string url)
        {
            return Task.Run(() =>
            {
                Driver.Navigate().GoToUrl(url);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(Driver.PageSource);

                var scraper = new HaloOglasiScraper();

                return scraper.ScrapeLinks(htmlDocument);
            });
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

        private void ReadRealEstateLinks()
        {
            using (FileStream fs = File.OpenRead(RealEstateLinksFilePath))
            {
                var pages = File.ReadLines(RealEstateLinksFilePath);

                foreach (var page in pages)
                {
                    RealEstatePages.Add(page);
                }

                Console.Write("Loaded all");
            }
        }
    }
}
