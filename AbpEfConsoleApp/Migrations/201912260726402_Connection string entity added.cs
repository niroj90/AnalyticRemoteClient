namespace AbpEfConsoleApp.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Connectionstringentityadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConnectionStrings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DatabaseName = c.String(nullable: false, maxLength: 50),
                        ConnectionStringValue = c.String(maxLength: 200),
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
                    { "DynamicFilter_ConnectionString_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ConnectionStrings",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ConnectionString_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
