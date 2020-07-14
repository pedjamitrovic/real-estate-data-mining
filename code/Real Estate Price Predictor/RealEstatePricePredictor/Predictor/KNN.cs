using System;
using System.Collections.Generic;

namespace RealEstatePricePredictor
{
    public class KNN
    {
        public int K;

        public List<Instance> Train;
        public List<Instance> Test;
        public Preprocessor P;

        private readonly Metrics Metrics = new Metrics();
        private readonly int ClassCount = 5;

        public KNN(Preprocessor p)
        {
            Train = p.Train;
            Test = p.Test;
            P = p;

            K = (int)Math.Sqrt(Train.Count);
            if (K % 2 == 0) { K += 1; }
        }

        public KNN(int k, Preprocessor p) : this(p)
        {
            if (k > 0)
            {
                K = k;
                if (K % 2 == 0) { K += 1; }
            }
        }

        public void Fit()
        {
        }

        public List<int> Predict(List<Instance> instances)
        {
            List<int> predictions = new List<int>();
            foreach (var instance in instances)
            {
                predictions.Add(Predict(instance));
            }
            return predictions;
        }

        public int Predict(Instance instance)
        {
            List<Tuple<Instance, double>> distances = new List<Tuple<Instance, double>>();

            foreach (var currInstance in Train)
            {
                distances.Add(new Tuple<Instance, double>(currInstance, Distance(instance, currInstance)));
            }

            distances.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            int[] classes = new int[ClassCount];

            for (int i = 0; i < K; ++i)
            {
                ++classes[distances[i].Item1.PriceRange];
            }

            int maxClassCount = 0, predictedClass = 0;
            for (int i = 0; i < ClassCount; ++i)
            {
                int classCount = classes[i];
                if (classCount > maxClassCount)
                {
                    maxClassCount = classCount;
                    predictedClass = i;
                }
            }

            return predictedClass;
        }

        public double Distance(Instance first, Instance second)
        {
            return ManhattanDistance(first, second);
        }

        public double EuclideanDistance(Instance first, Instance second)
        {
            double distance = 0;

            for (int i = 1; i < first.Data.Length; ++i)
            {
                distance += Math.Pow(first.Data[i] - second.Data[i], 2);
            }

            return Math.Sqrt(distance);
        }

        public double ManhattanDistance(Instance first, Instance second)
        {
            double distance = 0;

            for (int i = 1; i < first.Data.Length; ++i)
            {
                distance += Math.Abs(first.Data[i] - second.Data[i]);
            }

            return distance;
        }

        public double ChebyshevDistance(Instance first, Instance second)
        {
            double distance = 0;

            for (int i = 1; i < first.Data.Length; ++i)
            {
                var currDimDistance = Math.Abs(first.Data[i] - second.Data[i]);
                if (currDimDistance > distance)
                {
                    distance = currDimDistance;
                }
            }

            return distance;
        }

        public void PredictAndLogTestResults(int logCount = 50)
        {
            Console.WriteLine("Test prediction started - KNN");
            var predictions = Predict(Test);
            Console.WriteLine($"Test -> K = {K} Macro Average = {Metrics.MacroAverage(predictions, Test).ToString("0.###")}");
            if (logCount > 0) Console.WriteLine($"First {logCount} predictions:");
            for (int i = 0; i < logCount; ++i)
            {
                Console.WriteLine($"[{i + 1}] -> Predicted: {predictions[i]} / Expected: {Test[i].PriceRange}");
            }
            Console.WriteLine("Test prediction finished - KNN");
        }
    }
}
