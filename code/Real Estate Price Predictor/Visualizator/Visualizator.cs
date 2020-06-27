using RealEstatePricePredictor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Visualizator
{
    public partial class Visualizator : Form
    {
        public Visualizator()
        {
            InitializeComponent();
        }

        private void Visualizator_Load(object sender, EventArgs e)
        {
            using (var db = new RealEstateContext())
            {
                InitChartA(db);
                InitChartB(db);
                InitChartC(db);
                InitChartD(db);
                InitChartE(db);
            }
        }

        private void InitChartA(RealEstateContext db)
        {
            var query = from re in db.RealEstates
                        where re.City == "Beograd"
                        group re by re.Neighborhood into g
                        select new { Neighborhood = g.Key, TotalCount = g.Count() } into a
                        orderby a.TotalCount descending
                        select a;

            foreach (var item in query.Take(8).ToList())
            {
                ChartA.Series["Broj nekretnina"].Points.AddXY(item.Neighborhood, item.TotalCount);
            }
        }

        private void InitChartB(RealEstateContext db)
        {
            var flatsForSale = db.RealEstates.Where(re => re.HousingType == 0 && re.OfferType == 0);

            ChartB.Series["Broj stanova"].Points.AddXY("-35", flatsForSale.Where(re => re.Quadrature <= 35).Count());
            ChartB.Series["Broj stanova"].Points.AddXY("36-50", flatsForSale.Where(re => re.Quadrature > 35 && re.Quadrature <= 50).Count());
            ChartB.Series["Broj stanova"].Points.AddXY("51-65", flatsForSale.Where(re => re.Quadrature > 50 && re.Quadrature <= 65).Count());
            ChartB.Series["Broj stanova"].Points.AddXY("66-80", flatsForSale.Where(re => re.Quadrature > 65 && re.Quadrature <= 80).Count());
            ChartB.Series["Broj stanova"].Points.AddXY("81-95", flatsForSale.Where(re => re.Quadrature > 80 && re.Quadrature <= 95).Count());
            ChartB.Series["Broj stanova"].Points.AddXY("96-110", flatsForSale.Where(re => re.Quadrature > 95 && re.Quadrature <= 110).Count());
            ChartB.Series["Broj stanova"].Points.AddXY("111+", flatsForSale.Where(re => re.Quadrature > 110).Count());
        }

        private void InitChartC(RealEstateContext db)
        {
            // ChartC.Series["Broj stanova"].Points.AddXY("1950-1959", db.RealEstates.Where((re) => re.YearBuilt >= 1950 && re.YearBuilt < 1960).Count());
            // ChartC.Series["Broj stanova"].Points.AddXY("1960-1969", db.RealEstates.Where((re) => re.YearBuilt >= 1960 && re.YearBuilt < 1970).Count());
            // ChartC.Series["Broj stanova"].Points.AddXY("1970-1979", db.RealEstates.Where((re) => re.YearBuilt >= 1970 && re.YearBuilt < 1980).Count());
            // ChartC.Series["Broj stanova"].Points.AddXY("1980-1989", db.RealEstates.Where((re) => re.YearBuilt >= 1980 && re.YearBuilt < 1990).Count());
            // ChartC.Series["Broj stanova"].Points.AddXY("1990-1999", db.RealEstates.Where((re) => re.YearBuilt >= 1990 && re.YearBuilt < 2000).Count());
            // ChartC.Series["Broj stanova"].Points.AddXY("2000-2009", db.RealEstates.Where((re) => re.YearBuilt >= 2000 && re.YearBuilt < 2010).Count());
            // ChartC.Series["Broj stanova"].Points.AddXY("2010-2019", db.RealEstates.Where((re) => re.YearBuilt >= 2010 && re.YearBuilt < 2020).Count());
        }

        private void InitChartD(RealEstateContext db)
        {
            var citiesByEstateCount = from re in db.RealEstates
                                      group re by re.City into g
                                      select new { Name = g.Key, TotalCount = g.Count() } into a
                                      orderby a.TotalCount descending
                                      select a;
            var topFiveCities = citiesByEstateCount.Take(5).ToList();

            int i = 1;

            foreach (var city in topFiveCities)
            {
                var query = db.RealEstates.Where(re => re.City == city.Name);
                decimal sellingCount = query.Where(re => re.OfferType == OfferType.Selling).Count();
                decimal rentalCount = query.Where(re => re.OfferType == OfferType.Rental).Count();

                Chart chart = GetChartD(i++);
                chart.Titles.Add("Title").Text = $"{city.Name} - odnos broja nekretnina";
                chart.Series["Broj nekretnina"].Points.AddXY("Prodaja", sellingCount);
                chart.Series["Broj nekretnina"].Points[0].Label = string.Format("{0} ({1:p})", sellingCount, sellingCount / (sellingCount + rentalCount));
                chart.Series["Broj nekretnina"].Points.AddXY("Iznajmljivanje", rentalCount);
                chart.Series["Broj nekretnina"].Points[1].Label = string.Format("{0} ({1:p})", rentalCount, rentalCount / (sellingCount + rentalCount));
            }
        }

        private Chart GetChartD(int i)
        {
            switch (i)
            {
                case 1:
                    return ChartD1;
                case 2:
                    return ChartD2;
                case 3:
                    return ChartD3;
                case 4:
                    return ChartD4;
                case 5:
                    return ChartD5;
                default:
                    return ChartD1;
            }
        }

        private void InitChartE(RealEstateContext db)
        {
            var realEstatesForSale = db.RealEstates.Where(re => re.OfferType == 0);
            decimal totalCount = realEstatesForSale.Count();

            decimal count = realEstatesForSale.Where(re => re.Price < 50000).Count();
            ChartE.Series["Broj nekretnina"].Points.AddXY("-49999", count);
            ChartE.Series["Broj nekretnina"].Points[0].Label = string.Format("{0} ({1:p})", count, count / totalCount);

            count = realEstatesForSale.Where(re => re.Price >= 50000 && re.Price < 100000).Count();
            ChartE.Series["Broj nekretnina"].Points.AddXY("50000-99999", count);
            ChartE.Series["Broj nekretnina"].Points[1].Label = string.Format("{0} ({1:p})", count, count / totalCount);

            count = realEstatesForSale.Where(re => re.Price >= 100000 && re.Price < 150000).Count();
            ChartE.Series["Broj nekretnina"].Points.AddXY("100000-149999", count);
            ChartE.Series["Broj nekretnina"].Points[2].Label = string.Format("{0} ({1:p})", count, count / totalCount);

            count = realEstatesForSale.Where(re => re.Price >= 150000 && re.Price < 200000).Count();
            ChartE.Series["Broj nekretnina"].Points.AddXY("150000-199999", count);
            ChartE.Series["Broj nekretnina"].Points[3].Label = string.Format("{0} ({1:p})", count, count / totalCount);

            count = realEstatesForSale.Where(re => re.Price >= 200000).Count();
            ChartE.Series["Broj nekretnina"].Points.AddXY("200000+", count);
            ChartE.Series["Broj nekretnina"].Points[4].Label = string.Format("{0} ({1:p})", count, count / totalCount);
        }
    }
}
