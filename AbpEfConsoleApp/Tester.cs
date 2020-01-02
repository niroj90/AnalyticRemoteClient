using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
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

        public Tester()
        {
            Logger = NullLogger.Instance;
        }

        public void Run()
        {
            Logger.Debug("Started Tester.Run()");
        }


        public void StartSharding(DateTime from,DateTime to)
        {
            Console.WriteLine("Starting the sharding process");
            string connectionString = string.Empty;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    string dailyQuery = GetQueryForDaily(from, to);
                    DataTable dailyDatatable = GetRecords(connectionString, dailyQuery);
                    long remoteClientId = 1;
                    ShardData(dailyDatatable, Periodicity._1, remoteClientId);
                }
                else
                {
                    Console.WriteLine("Connection string not found.Aborting...");
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception happens while getting connection string from db" + ex.Message);
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

        private string GetQueryForDaily(DateTime from,DateTime to)
        {
            string query = @"select 
                                MAX(Organizations.Id) as OrganizationId,
                                MAX(Organizations.DisplayName) as OrganizationName,
                                MAX(Departments.Id) as DepartmentId, 
                                MAX(Departments.DisplayName) as DepartmentName,
                                max(Date) as Date, SUM(Earning) as Sum, AVG(Earning) as Average, COUNT(*) as Count
                                from Revenues
                                inner join Departments on Revenues.DepartmentId = Departments.Id
                                inner join Organizations on Departments.OrganizationId = Organizations.Id
                                group by Date";
            return query;
        }

        private bool ShardData(DataTable dt, Periodicity periodicity,long remoteClientId)
        {
            bool isSuccessFull = false;
            try
            {
                if (dt!=null && dt.Rows.Count>0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var analyticData = MapList<AnalyticsInputDto>(dt.Rows[i]);
                        try
                        {
                            analyticData.RemoteClientId = remoteClientId;
                            analyticData.Periodicity = periodicity;
                            analyticData.Average = Convert.ToDouble(dt.Rows[i]["Average"]);
                            analyticData.Sum = Convert.ToDouble(dt.Rows[i]["Sum"]);
                            analyticData.Count = Convert.ToInt32(dt.Rows[i]["Count"]);
                            analyticData.Date = Convert.ToDateTime(dt.Rows[i]["Date"]);
                            analyticData.IsHoliday = Convert.ToBoolean(dt.Rows[i]["IsHoliday"]);
                            analyticData.IsRainy = Convert.ToBoolean(dt.Rows[i]["IsRainy"]);
                            Console.WriteLine($"Sending data to server for Organization : {analyticData.OrganizationName} Department : {analyticData.DepartmentName} Date : {analyticData.Date}");
                            SendDataToServer(analyticData);
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine($"Failed to send data to server for Organization : {analyticData.OrganizationName} Department : {analyticData.DepartmentName} Date : {analyticData.Date}");
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

        private void SendDataToServer(AnalyticsInputDto input)
        {
            var httpClient = new HttpClient();
            var client = new Client("http://localhost:21021/", httpClient);
            try
            {
                client.CreateOrUpdateAsync(input);
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}