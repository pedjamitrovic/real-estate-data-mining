using HtmlAgilityPack;
using Real_Estate_Price_Predictor.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Real_Estate_Price_Predictor
{
    class HaloOglasiScraper
    {
        private static string[] floorLiterals = { "sut", "psut", "vpr", "pr" };
        public List<string> ScrapeLinks(HtmlDocument document)
        {
            List<string> links = new List<string>();
            foreach (var node in document.DocumentNode.SelectNodes("//h3[@class='product-title']"))
            {
                links.Add(node.FirstChild.GetAttributeValue("href", ""));
            }
            return links;
        }
 
        public RealEstate ScrapeRealEstate(HtmlDocument document)
        {
            var re = new RealEstate();
            
            var priceNode = document.DocumentNode.SelectSingleNode("//span[@class=\"offer-price-value\"]");
            if (priceNode != null && int.TryParse(priceNode.GetDirectInnerText().Replace(".", ""), out int price))
            {
                re.Price = price;
            }

            var breadcrumbNode = document.DocumentNode.SelectSingleNode("//ol[@id=\"main-breadcrumb\"]");
            var adInfo = breadcrumbNode.Descendants("span").Skip(2).First().GetDirectInnerText();
            re.OfferType = adInfo.Split(" ")[0].Equals("prodaja", StringComparison.OrdinalIgnoreCase) ? OfferType.Selling : OfferType.Rental;
            re.HousingType = adInfo.Split(" ")[1].Equals("stanova", StringComparison.OrdinalIgnoreCase) ? HousingType.Apartment : HousingType.House;

            var cityNode = document.DocumentNode.SelectSingleNode("//span[@id=\"plh2\"]");
            if (cityNode != null) re.City = cityNode.GetDirectInnerText();

            var neighborhoodNode = document.DocumentNode.SelectSingleNode("//span[@id=\"plh4\"]");
            if (neighborhoodNode != null) re.Neighborhood = neighborhoodNode.GetDirectInnerText();

            var quadratureNode = document.DocumentNode.SelectSingleNode("//span[@id=\"plh11\"]");
            if (quadratureNode != null && int.TryParse(quadratureNode.GetDirectInnerText().Split(" ")[0], out int quadrature))
            {
                re.Quadrature = quadrature;
            }

            var roomCountNode = document.DocumentNode.SelectSingleNode("//span[@id=\"plh12\"]");
            if (roomCountNode != null && double.TryParse(roomCountNode.GetDirectInnerText().Replace("+", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out double roomCount))
            {
                re.RoomCount = roomCount;
            }

            var heatingTypeNode = document.DocumentNode.SelectSingleNode("//span[@id=\"plh17\"]");
            if (heatingTypeNode != null) re.HeatingType = heatingTypeNode.GetDirectInnerText();

            var floorNode = document.DocumentNode.SelectSingleNode("//span[@id=\"plh18\"]");
            if (floorNode != null)
            {
                var floorString = floorNode.GetDirectInnerText();
                if (floorLiterals.Contains(floorString))
                {
                    floorString = "0";
                }
                if (int.TryParse(floorString, out int floor)) re.Floor = floor;
            }

            var totalFloorsNode = document.DocumentNode.SelectSingleNode("//span[@id=\"plh19\"]");
            if (totalFloorsNode != null)
            {
                var totalFloorsString = totalFloorsNode.GetDirectInnerText();
                if (floorLiterals.Contains(totalFloorsString))
                {
                    totalFloorsString = "0";
                }
                if (int.TryParse(totalFloorsString, out int totalFloors)) re.TotalFloors = totalFloors;
            }

            var landSizeNode = document.DocumentNode.SelectSingleNode("//span[@id=\"plh21\"]");
            if(landSizeNode != null && double.TryParse(landSizeNode.GetDirectInnerText().Split(" ")[0].Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double landSize))
            {
                re.LandSize = landSize;
            }

            var registeredNode = document.DocumentNode.SelectSingleNode("//span[@id=\"plh22\"]");
            if (registeredNode != null) re.Registered = true;
            
            return re;
        } 
    }
}
