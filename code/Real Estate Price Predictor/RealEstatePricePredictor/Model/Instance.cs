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
