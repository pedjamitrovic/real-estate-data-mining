using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Text;

namespace RealEstatePricePredictor
{
    class HaloOglasiCrawler : IDisposable
    {
        public List<string> RealEstatePages { get; } = new List<string>();
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
                ReadRealEstateLinks();
                using (var db = new RealEstateContext())
                {
                    var errorCount = 0;
                    for (int i = 22001; i < RealEstatePages.Count; ++i)
                    {
                        try
                        {
                            var re = await ParseRealEstate(RealEstatePages[i]);
                            if (re != null)
                            {
                                Console.WriteLine($"{i} -> {re.Url} OK");
                                db.RealEstates.Add(re);
                            }
                            else
                            {
                                Console.WriteLine($"{i} -> {RealEstatePages[i]} NOT FOUND");
                            }
                        }
                        catch (Exception)
                        {
                            ++errorCount;
                            Console.WriteLine($"{i} -> {RealEstatePages[i]} ERROR");
                        }
                        finally
                        {
                            if (i % 100 == 0)
                            {
                                db.SaveChanges();
                                Console.WriteLine($"SAVING CHANGES TO DATABASE");
                            }
                            UseNewProxy();
                        }
                    }
                    Console.WriteLine("Finished. Error count = " + errorCount);
                }
            });
        }

        public void Dispose()
        {
            try
            {
                Driver.Close();
            }
            catch (Exception)
            {

            }
            finally
            {
                Driver.Quit();
            }
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
                try
                {
                    Driver.Navigate().GoToUrl(url);

                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(Driver.PageSource);

                    var scraper = new HaloOglasiScraper();

                    var realEstate = scraper.ScrapeRealEstate(htmlDocument);

                    if (realEstate != null) realEstate.Url = url;

                    return realEstate;
                }
                catch (Exception)
                {
                    return null;
                }
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

                Console.WriteLine("Real estate links loaded");
            }
        }
    }
}
