using System;
using System.Collections.Generic;

namespace RealEstatePricePredictor
{
    public class LinearRegression
    {
        public int Iterations;
        public int Steps;
        public double L;

        public double[] W;
        public List<Instance> Train;
        public List<Instance> Test;

        private readonly int N; // Num of features + 1
        private readonly Metrics Metrics = new Metrics();

        public LinearRegression(int iterations, int steps, double learningRate, List<Instance> train, List<Instance> test)
        {
            Iterations = iterations;
            Steps = steps;
            L = learningRate;

            Train = train;
            Test = test;

            N = Train[0].Data.Length;

            Random rnd = new Random();
            W = new double[N];
            for (int i = 0; i < N; ++i)
            {
                W[i] = rnd.NextDouble();
            }
        }

        public void Fit()
        {
            Console.WriteLine("Training started");
            for (int i = 1; i < Iterations + 1; ++i)
            {
                List<double> predictions = Predict(Train);
                UpdateW(predictions);
                //if (i % Steps == 0)
                //{
                //    ComputeCost(predictions);
                //    Console.Write($"Iteration {i} elapsed -> Cost function value = {ComputeCost(predictions).ToString("0.##")} -> RMSE = {Metrics.RMSE(predictions, Train).ToString("0.##")} -> W[] = [ ");
                //    foreach(var w in W) {
                //        Console.Write($"{w.ToString("0.##")} ");
                //    }
                //    Console.WriteLine("]");
                //}
            }
            Console.WriteLine("Training finished");
        }

        public List<double> Predict(List<Instance> instances)
        {
            List<double> predictions = new List<double>();
            foreach (var instance in instances)
            {
                predictions.Add(Predict(instance));
            }
            return predictions;
        }

        public double Predict(Instance instance)
        {
            double prediction = W[0];
            for (int j = 1; j < N; ++j)
            {
                prediction += W[j] * instance.Data[j];
            }
            return prediction;
        }

        public void PredictAndLogTestResults(int logCount = 50)
        {
            Console.WriteLine("Test prediction started");
            var predictions = Predict(Test);
            Console.WriteLine($"Test -> RMSE = {Metrics.RMSE(predictions, Test).ToString("0.##")}");
            Console.WriteLine($"First {logCount} predictions:");
            for (int i = 0; i < logCount; ++i)
            {
                Console.WriteLine($"[{i + 1}] -> Predicted: {predictions[i].ToString("0.##")} / Expected: {Test[i].Data[0].ToString("0.##")}");
            }
            Console.WriteLine("Test prediction finished");
        }

        private void UpdateW(List<double> predictions)
        {
            double[] sums = new double[N];
            int m = predictions.Count;

            for (int i = 0; i < m; ++i)
            {
                double predicted = predictions[i];
                double expected = Train[i].Data[0];

                for (int j = 0; j < N; ++j)
                {
                    if (j == 0)
                    {
                        sums[j] += (predicted - expected);
                    }
                    else
                    {
                        sums[j] += (predicted - expected) * Train[i].Data[j];
                    }
                }
            }

            for (int j = 0; j < N; ++j)
            {
                W[j] = W[j] - (L / m) * (sums[j]);
            }
        }

        private double ComputeCost(List<double> predictions)
        {
            double sum = 0;
            int m = predictions.Count;

            for (int i = 0; i < m; ++i)
            {
                double predicted = predictions[i];
                double expected = Train[i].Data[0];
                sum += Math.Pow(predicted - expected, 2);
            }

            return sum / (2 * m);
        }
    }
}
