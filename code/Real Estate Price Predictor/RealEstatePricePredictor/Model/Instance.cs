using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstatePricePredictor
{
    public class Instance
    {
        public double Distance;
        public double Quadrature;
        public double Registered;
        public double Floor;
        public double RoomCount;

        public double Price;

        public new String ToString()
        {
            return $"{Distance.ToString("0.##")} {Quadrature} {Registered} {Floor} {RoomCount} {Price}";
        }
    }
}
