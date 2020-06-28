namespace RealEstatePricePredictor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RealEstates",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Price = c.Int(nullable: false),
                        HousingType = c.Int(nullable: false),
                        OfferType = c.Int(nullable: false),
                        City = c.String(),
                        Neighborhood = c.String(),
                        Quadrature = c.Int(nullable: false),
                        YearBuilt = c.Int(),
                        LandSize = c.Int(),
                        Floor = c.Int(),
                        TotalFloors = c.Int(),
                        Registered = c.Boolean(nullable: false),
                        HeatingType = c.String(),
                        RoomCount = c.Double(),
                        BathroomCount = c.Double(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RealEstates");
        }
    }
}
