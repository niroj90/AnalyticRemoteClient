using System.Data.Entity;
using Abp.EntityFramework;
using AbpEfConsoleApp.Entities;

namespace AbpEfConsoleApp
{
    //EF DbContext class.
    public class MyConsoleAppDbContext : AbpDbContext
    {
        public virtual IDbSet<ConnectionString> ConnectionStrings { get; set; }
        public virtual IDbSet<Analytics> Analytics { get; set; }
        public virtual IDbSet<TableReference> TableReferences { get; set; }


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