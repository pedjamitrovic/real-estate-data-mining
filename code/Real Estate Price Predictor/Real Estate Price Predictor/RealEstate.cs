using System;
using System.Collections.Generic;
using System.Text;

namespace Real_Estate_Price_Predictor
{
    public enum HousingType { Apartment, House };
    public enum OfferType { Selling, Rental };
    class RealEstate
    {
        public string Url { get; set; }
        public double Price { get; set; }
        public HousingType HousingType { get; set; }
        public OfferType OfferType { get; set; }
        public string City { get; set; }
        public string Neighborhood { get; set; }
        public int Quadrature { get; set; }
        public int YearBuilt { get; set; } // Nema
        public double LandSize { get; set; }
        public int Floor { get; set; }
        public int TotalFloors { get; set; }
        public bool Registered { get; set; }
        public string HeatingType { get; set; }
        public double RoomCount { get; set; }
        public double BathroomCount { get; set; } // Nema

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Url -> {Url}\n");
            sb.Append($"Price -> {Price}\n");
            sb.Append($"HousingType -> {HousingType}\n");
            sb.Append($"OfferType -> {OfferType}\n");
            sb.Append($"City -> {City}\n");
            sb.Append($"Neighborhood -> {Neighborhood}\n");
            sb.Append($"Quadrature -> {Quadrature}\n");
            sb.Append($"YearBuilt -> {YearBuilt}\n");
            sb.Append($"LandSize -> {LandSize}\n");
            sb.Append($"Floor -> {Floor}\n");
            sb.Append($"TotalFloors -> {TotalFloors}\n");
            sb.Append($"Registered -> {Registered}\n");
            sb.Append($"HeatingType -> {HeatingType}\n");
            sb.Append($"RoomCount -> {RoomCount}\n");
            sb.Append($"BathroomCount -> {BathroomCount}\n");
            return sb.ToString();
        }
    }
}
