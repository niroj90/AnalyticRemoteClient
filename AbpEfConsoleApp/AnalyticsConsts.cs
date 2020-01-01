using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpEfConsoleApp
{
    public class AnalyticsConsts
    {
        public static class MaxLengths
        {
            public const int DatabaseName = 50;
            public const int ConnectionString = 200;
            public const int TableName = 50;
            public const int ColumnName = 50;
            public const int MappedColumnName = 50;
        }

        public static class ColumnNames
        {
            public const string OrganizationId = "OrganizationId";
            public const string DepartmentId = "DepartmentId";
            public const string Earning = "Earning";
        }
    }
}
