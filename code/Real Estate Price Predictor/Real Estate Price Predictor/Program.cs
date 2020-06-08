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
            StartAsync();
            Console.ReadLine();
        }
        public static async void StartAsync()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 4; ++i)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using (var crawler = new HaloOglasiCrawler())
                    {
                        await crawler.StartCrawling();
                    }
                }));
            }
            await Task.WhenAll(tasks);
        }
    }
}
