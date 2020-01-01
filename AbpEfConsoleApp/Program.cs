using System;
using Abp;
using Abp.Dependency;
using Castle.Facilities.Logging;

namespace AbpEfConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Specify the database name to shard:");
            string dbName = Console.ReadLine();
            //Bootstrapping ABP system
            using (var bootstrapper = AbpBootstrapper.Create<MyConsoleAppModule>())
            {
                bootstrapper.IocManager
                    .IocContainer
                    .AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));

                bootstrapper.Initialize();

                //Getting a Tester object from DI and running it
                using (var tester = bootstrapper.IocManager.ResolveAsDisposable<Tester>())
                {
                    tester.Object.StartSharding(dbName, DateTime.Now,DateTime.Now);
                } //Disposes tester and all it's dependencies

                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }
    }
}
