using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstatePricePredictor
{
    public class Metrics
    {
        public double RMSE(List<double> predictions, List<Instance> set)
        {
            double sum = 0;
            int m = predictions.Count;

            for (int i = 0; i < m; ++i)
            {
                double predicted = predictions[i];
                double expected = set[i].Data[0];
                sum += Math.Pow(predicted - expected, 2);
            }

            return Math.Sqrt(sum / m);
        }

        public double MacroAverage(List<int> predictions, List<Instance> set){

            int m = predictions.Count;
            int n = 5;
            int[,] confusionMatrix = new int[n, n];

            for (int i = 0; i < m; ++i)
            {
                int predicted = predictions[i];
                int expected = set[i].PriceRange;
                ++confusionMatrix[expected, predicted];
            }

            double[] p = new double[n];
            double[] r = new double[n];

            for (int i = 0; i < n; ++i)
            {
                double sumP = 0, sumR = 0;

                for(int j = 0; j < n; ++j)
                {
                    sumP += confusionMatrix[j, i];
                    sumR += confusionMatrix[i, j];
                }

                p[i] = confusionMatrix[i, i] / sumP;
                r[i] = confusionMatrix[i, i] / sumR;
            }

            double pMacro = p.Sum() / n;
            double rMacro = r.Sum() / n;

            double fMacro = (2 * pMacro * rMacro) / (pMacro + rMacro); 

            return fMacro;
        }
    }
}
