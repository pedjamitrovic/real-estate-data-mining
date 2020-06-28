using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstatePricePredictor
{
    public class RealEstateContext: DbContext
    {
        public RealEstateContext(): base("name=RealEstateContext")
        {
                
        }

        public virtual DbSet<RealEstate> RealEstates { get; set; }
        public virtual DbSet<NeighborhoodDistance> NeighborhoodDistances { get; set; }
    }
}
