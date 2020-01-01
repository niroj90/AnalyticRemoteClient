using System.Data.Entity;
using Abp.EntityFramework;
using AbpEfConsoleApp.Entities;

namespace AbpEfConsoleApp
{
    //EF DbContext class.
    public class MyConsoleAppDbContext : AbpDbContext
    {

        public MyConsoleAppDbContext()
            : base("Default")
        {

        }

        public MyConsoleAppDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
    }
}