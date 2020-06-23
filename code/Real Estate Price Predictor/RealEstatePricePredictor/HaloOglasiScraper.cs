using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RealEstatePricePredictor
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
            
            var priceNode = document.DocumentNode.SelectSingleNode("//span[@class='offer-price-value']");
            if (priceNode != null && double.TryParse(priceNode.GetDirectInnerText(), NumberStyles.Any, CultureInfo.GetCultureInfo("DE-de"), out double price))
            {
                re.Price = Convert.ToInt32(price);
            }

            var breadcrumbNode = document.DocumentNode.SelectSingleNode("//ol[@id='main-breadcrumb']");
            var adInfo = breadcrumbNode.Descendants("span").Skip(2).First().GetDirectInnerText();
            re.OfferType = adInfo.Split(' ')[0].Equals("prodaja", StringComparison.OrdinalIgnoreCase) ? OfferType.Selling : OfferType.Rental;
            re.HousingType = adInfo.Split(' ')[1].Equals("stanova", StringComparison.OrdinalIgnoreCase) ? HousingType.Apartment : HousingType.House;

            var cityNode = document.DocumentNode.SelectSingleNode("//span[@id='plh2']");
            if (cityNode != null) re.City = cityNode.GetDirectInnerText();

            var neighborhoodNode = document.DocumentNode.SelectSingleNode("//span[@id='plh4']");
            if (neighborhoodNode != null) re.Neighborhood = neighborhoodNode.GetDirectInnerText();

            var prominentListItems = document.DocumentNode.SelectNodes("//div[@class='prominent']//li");

            if (prominentListItems == null) return null; // Oglas vise ne postoji

            foreach (var li in prominentListItems)
            {
                var fieldNameNode = li.SelectSingleNode("span[@class='field-name']");
                if (fieldNameNode != null)
                {
                    var fieldValueNode = li.SelectSingleNode("span[@class='field-value']/span");
                    var fieldName = fieldNameNode.GetDirectInnerText();
                    if (fieldValueNode != null)
                    {
                        switch (fieldName.ToLower())
                        {
                            case "kvadratura":
                                if (double.TryParse(fieldValueNode.GetDirectInnerText().Split(' ')[0], NumberStyles.Any, CultureInfo.GetCultureInfo("DE-de"), out double quadrature))
                                {
                                    re.Quadrature = Convert.ToInt32(quadrature);
                                }
                                break;
                            case "broj soba":
                                if (double.TryParse(fieldValueNode.GetDirectInnerText().Replace("+", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out double roomCount))
                                {
                                    re.RoomCount = roomCount;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            var basicViewDivs = document.DocumentNode.SelectNodes("//div[@class='basic-view']/div");

            foreach (var basicViewDiv in basicViewDivs)
            {
                var innerDivs = basicViewDiv.SelectNodes("div");
                var key = innerDivs[0]?.GetDirectInnerText();

                if (key != null)
                {
                    var valueNode = innerDivs[1].SelectSingleNode("span");
                    if (valueNode != null)
                    {
                        switch (key.ToLower())
                        {
                            case "grejanje":
                                re.HeatingType = valueNode.GetDirectInnerText();
                                break;
                            case "sprat":
                                var floorString = valueNode.GetDirectInnerText().ToLower();
                                if (floorLiterals.Contains(floorString))
                                {
                                    floorString = "0";
                                }
                                if (int.TryParse(floorString, out int floor)) re.Floor = floor;
                                break;
                            case "ukupna spratnost":
                                var totalFloorsString = valueNode.GetDirectInnerText().ToLower();
                                if (floorLiterals.Contains(totalFloorsString))
                                {
                                    totalFloorsString = "0";
                                }
                                if (int.TryParse(totalFloorsString, out int totalFloors)) re.TotalFloors = totalFloors;
                                break;
                            case "površina placa":
                                var value = valueNode.GetDirectInnerText();
                                if (value.Split(' ')[1].StartsWith("m"))
                                {
                                    double.TryParse(value.Split(' ')[0], NumberStyles.Any, CultureInfo.GetCultureInfo("DE-de"), out double landSize);
                                    re.LandSize = Convert.ToInt32(landSize);
                                }
                                else
                                {
                                    double.TryParse(value.Split(' ')[0], NumberStyles.Any, CultureInfo.GetCultureInfo("DE-de"), out double landSize);
                                    re.LandSize = Convert.ToInt32(landSize * 100);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            var flagNodes = document.DocumentNode.SelectNodes("//span[contains(@class, 'flag-attribute')]/label");
            if (flagNodes != null)
            {
                foreach (var flagNode in flagNodes)
                {
                    if (flagNode?.GetDirectInnerText().ToLower() == "uknjižen")
                    {
                        re.Registered = true;
                        break;
                    }
                }
            }
            return re;
        } 
    }
}
