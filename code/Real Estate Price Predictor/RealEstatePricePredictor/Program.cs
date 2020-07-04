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

            var preprocessor = new Preprocessor(0.2);

            // Phase 4
            // var model = new LinearRegression(500, 50, 1e-5, 0.2);
            // model.Fit();
            // model.PredictAndLogTestResults();
            // Console.ReadLine();

            // Phase 5
            // var model = new KNN(0.2);
            // model.Fit();
            // model.PredictAndLogTestResults();
            // Console.ReadLine();

            Console.WriteLine("Training models");
            var linearRegressionModel = new LinearRegression(500, 50, 1e-5, preprocessor.Train, preprocessor.Test);
            linearRegressionModel.Fit();
            var knnModel = new KNN(preprocessor.Train, preprocessor.Test);
            knnModel.Fit();
            Console.WriteLine("Models trained");

            while (true)
            {
                try
                {
                    Console.WriteLine("Real estate price predictor");
                    Console.WriteLine("1. Linear regression");
                    Console.WriteLine("2. K-Nearest neighbours");
                    Console.WriteLine("0. Exit");
                    Console.Write("Enter option: ");
                    string input = Console.ReadLine();
                    switch (Convert.ToInt32(input))
                    {
                        case 0: return;
                        case 1: LinearRegressionMenu(linearRegressionModel); break;
                        case 2: KNNMenu(knnModel); break;
                        default: continue;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }

        public static void LinearRegressionMenu(LinearRegression model)
        {
            Instance instance = CreateInstance();

            if (instance != null)
            {
                double predicted = model.Predict(instance);
                Console.WriteLine($"Predicted price -> {predicted}");
            }
        }

        public static void KNNMenu(KNN model)
        {
            try
            {
                Console.WriteLine($"Current K is {model.K}");
                Console.Write("Enter new K or 0 to continue with default value: ");
                int k = Convert.ToInt32(Console.ReadLine());
                if (k > 0)
                {
                    model.K = k;
                }

                Instance instance = CreateInstance();

                if (instance != null)
                {
                    int predicted = model.Predict(instance);

                    if (predicted == 0)
                    {
                        Console.WriteLine("Predicted class -> 0 <= Price < 50000");
                    }
                    else if (predicted == 1)
                    {
                        Console.WriteLine("Predicted class -> 50000 <= Price < 100000");
                    }
                    else if (predicted == 2)
                    {
                        Console.WriteLine("Predicted class -> 100000 <= Price < 150000");
                    }
                    else if (predicted == 3)
                    {
                        Console.WriteLine("Predicted class -> 150000 <= Price < 200000");
                    }
                    else Console.WriteLine("Predicted class -> 200000 <= Price");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input");
            }
        }

        public static Instance CreateInstance()
        {
            try
            {
                //public double Price;
                //public double Distance;
                //public double Quadrature;
                //public double Registered;
                //public double Floor;
                //public double RoomCount;

                Console.WriteLine("Enter distance, quadrature, registered (0 or 1), floor, room count separated by one space: ");
                string line = Console.ReadLine();
                string[] values = line.Split(' ');

                Instance instance = new Instance
                {
                    Data = new double[]
                        {
                            0,
                            Convert.ToDouble(values[0]),
                            Convert.ToDouble(values[1]),
                            Convert.ToDouble(values[2]),
                            Convert.ToDouble(values[3]),
                            Convert.ToDouble(values[4]),
                        }
                };

                Console.WriteLine($"Entered data -> [ D = {instance.Data[1]}, Q = {instance.Data[2]}, R = {instance.Data[3]}, F = {instance.Data[4]}, RC = {instance.Data[5]} ]");

                return instance;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input");
                return null;
            }
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
