using System;
using System.Collections.Generic;

namespace RealEstatePricePredictor
{
    public class LinearRegression
    {
        public int Iterations;
        public int Steps;
        public double LearningRate;
        public double[] W;
        public List<Instance> train;
        public List<Instance> test;

        public LinearRegression(int iterations, int steps, double learningRate, double testSize)
        {
            var preprocessor = new Preprocessor(testSize);
            train = preprocessor.train;
            test = preprocessor.test;
            Console.WriteLine(train[0].ToString());
            Console.WriteLine(test[0].ToString());
        }

        public void Fit()
        {
            Random rnd = new Random();
            const int N = 4;

            //We randomize the inital values of alpha and beta
            double theta1 = rnd.Next(0, 100);
            double theta2 = rnd.Next(0, 100);

            //Values of x, i.e the independent variable
            double[] x = new double[N] { 1, 2, 3, 4 };
            //VAlues of y, i.e the dependent variable
            double[] y = new double[N] { 5, 7, 9, 12 };
        }
    }
}
