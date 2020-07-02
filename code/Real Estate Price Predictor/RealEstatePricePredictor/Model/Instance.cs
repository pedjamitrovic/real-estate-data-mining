using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstatePricePredictor
{
    public class Instance
    {
        //public double Price;
        //public double Distance;
        //public double Quadrature;
        //public double Registered;
        //public double Floor;
        //public double RoomCount;
        public double[] Data;

        public int PriceRange
        {
            get
            {
                double val = Data[0];
                if (val < 50000)
                {
                    return 0;
                }
                else if (val < 100000)
                {
                    return 1;
                }
                else if (val < 150000)
                {
                    return 2;
                }
                else if (val < 200000)
                {
                    return 3;
                }
                else return 4;
            }
        }

        public new String ToString()
        {
            var sb = new StringBuilder();
            foreach (var field in Data)
            {
                sb.Append($"{field.ToString("0.##")} ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
