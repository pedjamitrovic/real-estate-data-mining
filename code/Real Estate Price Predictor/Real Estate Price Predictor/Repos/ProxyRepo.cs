using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Real_Estate_Price_Predictor.Repos
{
    public sealed class ProxyRepo
    {
        public static ProxyRepo Instance { get; } = new ProxyRepo();

        private static List<string> Buffer = new List<string>();
        private static int Pointer = 0;
        private static readonly ReaderWriterLockSlim rwLockSlim = new ReaderWriterLockSlim();
        private static readonly string apiUrl = "https://gimmeproxy.com/api/getProxy";
        private static readonly int FetchProxyCount = 9;
        private static readonly string FilePath = @"proxies.txt";

        static ProxyRepo()
        {
        }

        private ProxyRepo()
        {
        }

        public void Init()
        {
            if (File.Exists(FilePath))
            {
                Console.WriteLine($"File {FilePath} found. Started reading.");
                ReadBufferFromFile();
            }
            else
            {
                Console.WriteLine($"File {FilePath} not found. Started fetching from api.");
                InitApiFetch();
            }
        }

        public static string GetProxy()
        {
            try
            {
                rwLockSlim.EnterReadLock();
                if (Buffer.Count <= 0)
                {
                    throw new Exception("Proxy repo buffer is empty");
                }

                string proxy = Buffer[Pointer];
                Pointer = ++Pointer % Buffer.Count;

                return proxy;
            }
            finally
            {
                rwLockSlim.ExitReadLock();
            }
        }

        private async void InitApiFetch()
        {
            await FetchProxies();
            WriteBufferToFile();
        }

        private async Task FetchProxies()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    for (int i = 0; i < FetchProxyCount; ++i)
                    {
                        using (HttpResponseMessage res = await client.GetAsync(apiUrl))
                        {
                            using (HttpContent content = res.Content)
                            {
                                string data = await content.ReadAsStringAsync();
                                if (data != null)
                                {
                                    var dataObj = JObject.Parse(data);
                                    string proxy = dataObj["ipPort"]?.ToObject<string>();

                                    Buffer.Add(proxy);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void WriteBufferToFile()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            using (FileStream fs = File.Create(FilePath))
            {
                Buffer.ForEach((proxy) =>
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(proxy);
                    fs.Write(info, 0, info.Length);
                });
            }
        }

        private void ReadBufferFromFile()
        {
            using (FileStream fs = File.OpenRead(FilePath))
            {
                var proxies = File.ReadLines(FilePath);

                foreach (var proxy in proxies)
                {
                    Buffer.Add(proxy);
                }
            }
        }
    }
}
