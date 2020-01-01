namespace AbpEfConsoleApp.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class TableReferenceentityadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TableReferences",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConnectionStringId = c.Long(nullable: false),
                        TableName = c.String(nullable: false, maxLength: 50),
                        ColumnName = c.String(nullable: false, maxLength: 50),
                        MappedColumnName = c.String(nullable: false, maxLength: 50),
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
                    { "DynamicFilter_TableReference_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TableReferences",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TableReference_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
