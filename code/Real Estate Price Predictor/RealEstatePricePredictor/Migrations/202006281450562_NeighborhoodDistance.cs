namespace RealEstatePricePredictor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NeighborhoodDistance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NeighborhoodDistances",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Neighborhood = c.String(),
                        Lat = c.Double(nullable: false),
                        Lng = c.Double(nullable: false),
                        Distance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NeighborhoodDistances");
        }
    }
}
