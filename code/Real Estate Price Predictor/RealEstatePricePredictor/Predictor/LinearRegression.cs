using System;
using System.Collections.Generic;

namespace RealEstatePricePredictor
{
    public class LinearRegression
    {
        public int Steps;
        public int StepLogCount;
        public double L;

        public double[] W;
        public List<Instance> Train;
        public List<Instance> Test;
        public Preprocessor P;

        private readonly int N; // Num of features + 1
        private readonly Metrics Metrics = new Metrics();

        public LinearRegression(int steps, int stepLogCount, double learningRate, Preprocessor p)
        {
            Steps = steps;
            StepLogCount = stepLogCount;
            L = learningRate;
            P = p;

            Train = p.Train;
            Test = p.Test;

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
            Console.WriteLine("Training started - LR");
            for (int i = 1; i < Steps + 1; ++i)
            {
                List<double> predictions = Predict(Train);
                UpdateW(predictions);
                if (i % StepLogCount == 0)
                {
                    ComputeCost(predictions);
                    Console.Write($"Iteration {i} elapsed -> RMSE = {Metrics.RMSE(predictions, Train, P).ToString("0.")} -> W[] = [ ");
                    foreach (var w in W)
                    {
                        Console.Write($"{w.ToString("0.###")} ");
                    }
                    Console.WriteLine("]");
                }
            }
            Console.WriteLine("Training finished - LR");
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
            Console.WriteLine("Test prediction started - LR");
            var predictions = Predict(Test);
            Console.WriteLine($"Test -> RMSE = {Metrics.RMSE(predictions, Test, P).ToString("0.")}");
            if (logCount > 0) Console.WriteLine($"First {logCount} predictions:");
            for (int i = 0; i < logCount; ++i)
            {
                Console.WriteLine($"[{i + 1}] -> Predicted: {Math.Exp(P.DenormalizedPrice(predictions[i]) - 1).ToString("0.")} / Expected: {Math.Exp(P.DenormalizedPrice(Test[i].Data[0]) - 1).ToString("0.")}");
            }
            Console.WriteLine("Test prediction finished - LR");
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
