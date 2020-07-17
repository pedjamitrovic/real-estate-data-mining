using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstatePricePredictor
{
    public class LinearRegression
    {
        public int StepLogCount;
        public double L;

        public double[] W;
        public List<Instance> Train;
        public List<Instance> Validation;
        public List<Instance> Test;
        public Preprocessor P;

        private readonly int N; // Num of features + 1
        private readonly Metrics Metrics = new Metrics();

        public LinearRegression(int stepLogCount, double learningRate, Preprocessor p)
        {
            StepLogCount = stepLogCount;
            L = learningRate;
            P = p;

            Test = p.Test;

            Train = p.Train;
            Validation = p.Validation;
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
            var trainMax = double.PositiveInfinity;
            var valMax = double.PositiveInfinity;
            Console.WriteLine("Training started - LR");
            var i = 0;
            var errorRisingCount = 0;
            while(true)
            {
                List<double> valPred = Predict(Validation);
                List<double> trainPred = Predict(Train);

                UpdateW(trainPred);

                var valRMSE = Metrics.RMSE(valPred, Validation, P);
                var trainRMSE = Metrics.RMSE(trainPred, Train, P);

                if (++i % StepLogCount == 0)
                {
                    Console.Write($"Iteration {i} elapsed -> Train RMSE = {trainRMSE.ToString("0.")} | Val RMSE = {valRMSE.ToString("0.")} -> W[] = [ ");
                    foreach (var w in W)
                    {
                        Console.Write($"{w.ToString("0.###")} ");
                    }
                    Console.WriteLine("]");
                }
                if (valRMSE >= valMax || trainRMSE >= trainMax)
                {
                    if(++errorRisingCount > 10)
                    {
                        break; // Overfitting
                    }
                }
                else
                {
                    if (trainRMSE < trainMax)
                    {
                        trainMax = trainRMSE;
                    }
                    if (valRMSE < valMax)
                    {
                        valMax = valRMSE;
                    }
                    errorRisingCount = 0;
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
            Console.WriteLine($"Train - Val - Test");
            Console.WriteLine($"{Train.Count} - {Validation.Count} - {Test.Count}");
            double[] tr = new double[5];
            for (int i = 0; i < Train.Count; ++i)
            {
                ++tr[Train[i].PriceRange];
            }
            double[] val = new double[5];
            for (int i = 0; i < Validation.Count; ++i)
            {
                ++val[Validation[i].PriceRange];
            }
            double[] te = new double[5];
            for (int i = 0; i < Test.Count; ++i)
            {
                ++te[Test[i].PriceRange];
            }
            for (int i = 0; i < 5; ++i)
            {
                Console.WriteLine($"{tr[i]} - {val[i]} - {te[i]}");
            }
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
    }
}
