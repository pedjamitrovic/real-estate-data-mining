using System;
using System.Threading.Tasks;
using System.Device.Location;
using System.Globalization;

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
            //var preprocessor = new Preprocessor(0.2);
            //var model = new LinearRegression(50, 0.5, preprocessor);
            //model.Fit();
            //model.PredictAndLogTestResults();
            //Console.ReadLine();

            // Phase 5
            //var preprocessor = new Preprocessor(0.2);
            //var model = new KNN(1, preprocessor);
            //do
            //{
            //    model.PredictAndLogTestResults(0);
            //}
            //while (++model.K < 10);
            //Console.ReadLine();

            InteractWithUser();
        }

        public static void InteractWithUser()
        {
            var preprocessor = new Preprocessor(0.2);

            Console.WriteLine("Training models");
            var linearRegressionModel = new LinearRegression(50, 0.5, preprocessor);
            linearRegressionModel.Fit();
            linearRegressionModel.PredictAndLogTestResults(0);
            var knnModel = new KNN(preprocessor);
            knnModel.Fit();
            knnModel.PredictAndLogTestResults(0);
            Console.WriteLine("Models trained");

            while (true)
            {
                try
                {
                    Console.WriteLine();
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
                model.P.Normalize(instance);
                double predicted = model.Predict(instance);
                Console.WriteLine($"Predicted price -> {Math.Exp(model.P.DenormalizedPrice(predicted) - 1).ToString("0.")}");
            }
        }

        public static void KNNMenu(KNN model)
        {
            try
            {
                Console.WriteLine($"Current K is {model.K}");
                Console.Write("Enter new K or 0 to continue with current value: ");
                int k = Convert.ToInt32(Console.ReadLine());
                if (k > 0)
                {
                    model.K = k;
                }

                Instance instance = CreateInstance();

                if (instance != null)
                {
                    model.P.Normalize(instance);
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

                Console.WriteLine("Enter distance in km, quadrature, floor, room count separated by one space: ");
                string line = Console.ReadLine();
                string[] values = line.Split(' ');

                Instance instance = new Instance
                {
                    Data = new double[]
                        {
                            0,
                            Convert.ToDouble(values[0], CultureInfo.InvariantCulture),
                            Convert.ToDouble(values[1], CultureInfo.InvariantCulture),
                            Convert.ToDouble(values[2], CultureInfo.InvariantCulture),
                            Convert.ToDouble(values[3], CultureInfo.InvariantCulture),
                        }
                };

                Console.WriteLine($"Entered data -> [ D = {instance.Data[1]}, Q = {instance.Data[2]}, F = {instance.Data[3]}, RC = {instance.Data[4]} ]");

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
