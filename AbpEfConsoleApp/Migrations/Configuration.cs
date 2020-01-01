using System;

namespace AbpEfConsoleApp.Migrations
{
    using AbpEfConsoleApp.Entities;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<MyConsoleAppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MyConsoleAppDbContext context)
        {
            
            
        }
    }
}
