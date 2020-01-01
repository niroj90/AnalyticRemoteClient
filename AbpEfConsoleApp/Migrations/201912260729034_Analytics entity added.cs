namespace AbpEfConsoleApp.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Analyticsentityadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Analytics",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConnectionStringId = c.Long(nullable: false),
                        OrganizationId = c.Long(nullable: false),
                        DepartmentId = c.Long(nullable: false),
                        Periodicity = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Average = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Sum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Count = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Analytics_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConnectionStrings", t => t.ConnectionStringId, cascadeDelete: true)
                .Index(t => t.ConnectionStringId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Analytics", "ConnectionStringId", "dbo.ConnectionStrings");
            DropIndex("dbo.Analytics", new[] { "ConnectionStringId" });
            DropTable("dbo.Analytics",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Analytics_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
