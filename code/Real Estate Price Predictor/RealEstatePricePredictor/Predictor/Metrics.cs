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
    }
}
