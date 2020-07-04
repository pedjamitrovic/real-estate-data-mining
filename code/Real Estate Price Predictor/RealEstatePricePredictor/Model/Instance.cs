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
        //public double Floor;
        //public double RoomCount;
        public double[] Data;

        public double OriginalPrice;

        public int PriceRange
        {
            get
            {
                if (OriginalPrice < 50000)
                {
                    return 0;
                }
                else if (OriginalPrice < 100000)
                {
                    return 1;
                }
                else if (OriginalPrice < 150000)
                {
                    return 2;
                }
                else if (OriginalPrice < 200000)
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
