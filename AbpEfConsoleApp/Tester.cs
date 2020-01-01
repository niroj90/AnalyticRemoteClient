using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AbpEfConsoleApp.Entities;
using AbpEfConsoleApp.Enums;
using Castle.Core.Logging;

namespace AbpEfConsoleApp
{
    //Entry class of the test. It uses constructor-injection to get a repository and property-injection to get a Logger.
    public class Tester : ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IRepository<ConnectionString,long> _connectionstringRepository;
        private readonly IRepository<TableReference, long> _tableReferenceRepository;
        private readonly IRepository<Analytics, long> _analyticsRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public Tester(IRepository<ConnectionString, long> connectionstringRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TableReference, long> tableReferenceRepository,
            IRepository<Analytics, long> analyticsRepository)
        {
            _connectionstringRepository = connectionstringRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _tableReferenceRepository = tableReferenceRepository;
            _analyticsRepository = analyticsRepository;


            Logger = NullLogger.Instance;
        }

        public void Run()
        {
            Logger.Debug("Started Tester.Run()");

            
        }


        public void StartSharding( string dbname,DateTime from,DateTime to)
        {
            if (!string.IsNullOrEmpty(dbname))
            {
                Console.WriteLine("Starting the sharding operation for database :"+dbname);
                string connectionString = string.Empty;
                using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        Console.WriteLine("Getting Connection string value for database :" + dbname);
                        var conStrRecord= _connectionstringRepository.GetAll().Where(x => x.DatabaseName.ToLower() == dbname.ToLower()).FirstOrDefault();
                        if (conStrRecord!=null && !string.IsNullOrEmpty(conStrRecord.ConnectionStringValue))
                        {
                            Console.WriteLine("Connection string value found for database :" + dbname);
                            connectionString = conStrRecord.ConnectionStringValue;
                            Console.WriteLine("Getting table reference for database :" + dbname);
                            var columnsDetails = _tableReferenceRepository.GetAll().Where(x => x.ConnectionStringId == conStrRecord.Id);
                            if (columnsDetails!=null && columnsDetails.Count()>0)
                            {
                                Console.WriteLine("Getting table reference found for database :" + dbname);
                                string organizationIdColumn = columnsDetails.Where(x => x.ColumnName.ToLower() == AnalyticsConsts.ColumnNames.OrganizationId).FirstOrDefault().MappedColumnName;
                                string departmentIdColumn = columnsDetails.Where(x => x.ColumnName.ToLower() == AnalyticsConsts.ColumnNames.DepartmentId).FirstOrDefault().MappedColumnName;
                                string earningColumn = columnsDetails.Where(x => x.ColumnName.ToLower() == AnalyticsConsts.ColumnNames.Earning).FirstOrDefault().MappedColumnName;

                                string dailyQuery = GetQueryForDaily(organizationIdColumn, departmentIdColumn, earningColumn, from, to);
                                DataTable dailyDatatable = GetRecords(connectionString, dailyQuery);
                                ShardData(dailyDatatable,Periodicity.Daily, conStrRecord.Id);
                            }
                            else
                            {
                                Console.WriteLine("No column reference found for the dbname :" + dbname);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No connection string found for the dbname :" +dbname);
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Exception happens while getting connection string from db" + ex.Message);
                    }
                    uow.Complete();
                }
            }
            else
            {
                Console.WriteLine("Dbname is empty.Aborting the sharding operation");
            }
        }

        private DataTable GetRecords(string connectionString, string query)
        {
            int recCount = 0;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet recDataSet = new DataSet();
            DataTable recDataTable = new DataTable();
            try
            {
                Console.WriteLine("Starting to get records");
                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    SqlCommand recordsCmd = new SqlCommand(query, connection);
                    recordsCmd.CommandTimeout = 300;
                    connection.Open();
                    da.SelectCommand = recordsCmd;
                    da.Fill(recDataSet,"RevenueTable");
                    recDataTable = recDataSet.Tables["RevenueTable"];
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception happend while getting the records : "+ex.Message);
            }
            return recDataTable;
        }

        private string GetQueryForDaily(string organizationColumn,string departmentColum,string earningColumn,DateTime from,DateTime to)
        {
            string query = @"select max("+ organizationColumn + @") as OrganizationId,max("+ departmentColum + @") as DepartmentId,max(Date) as Date, SUM("+ earningColumn + @") as Sum, AVG("+ earningColumn + @") as Average, COUNT(*) as Count
                            from Revenues
                            group by Date";
            return query;
        }

        private bool ShardData(DataTable dt, Periodicity periodicity,long connectionStringId)
        {
            bool isSuccessFull = false;
            try
            {
                if (dt!=null && dt.Rows.Count>0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var analyticData = MapList<Analytics>(dt.Rows[i]);
                        analyticData.Periodicity = periodicity;
                        analyticData.ConnectionStringId = connectionStringId;
                        analyticData.CreationTime = DateTime.Now;
                        var existingRecord = _analyticsRepository.GetAll().Where(x => x.ConnectionStringId == connectionStringId
                                            && x.Periodicity == periodicity
                                            && DbFunctions.TruncateTime(x.Date) == analyticData.Date.Date
                                            && x.OrganizationId == analyticData.OrganizationId
                                            && x.DepartmentId == analyticData.DepartmentId).FirstOrDefault();
                        if (existingRecord==null)
                        {
                            _analyticsRepository.Insert(analyticData);
                            Console.WriteLine($"Inserting analytic record for ConnectionStringId:{connectionStringId} OrganizationId:{analyticData.OrganizationId} DepartmentId:{analyticData.DepartmentId} Periodicity:{periodicity.ToString()} Date:{analyticData.Date.ToString()}");
                        }
                        else
                        {
                            existingRecord.Sum = analyticData.Sum;
                            existingRecord.Average = analyticData.Average;
                            existingRecord.Count = analyticData.Count;
                            existingRecord.LastModificationTime = DateTime.Now;
                            _analyticsRepository.Update(existingRecord);
                            Console.WriteLine($"Updating analytic record for ConnectionStringId:{connectionStringId} OrganizationId:{analyticData.OrganizationId} DepartmentId:{analyticData.DepartmentId} Periodicity:{periodicity.ToString()} Date:{analyticData.Date.ToString()}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception happens while sharding the data :"+ex.Message );
            }
            return isSuccessFull;
        }

        private static T MapList<T>(DataRow dr)
        {
            T result = Activator.CreateInstance<T>();

            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                

                foreach (PropertyInfo pr in properties)
                {
                    if (dr.Table.Columns.Contains(pr.Name))
                    {
                        pr.SetValue(result, dr[pr.Name]);
                    }
                    
                }
                    
            }
            catch (Exception ex)
            {

                Console.WriteLine("Execption while converting the data row to object :" + ex.Message);
            }

            return result;
        }

    }
}