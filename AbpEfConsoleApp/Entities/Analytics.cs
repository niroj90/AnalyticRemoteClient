using Abp.Domain.Entities.Auditing;
using AbpEfConsoleApp.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpEfConsoleApp.Entities
{
    public class Analytics 
    {
        public long RemoteClientId { get; set; }
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Periodicity Periodicity { get; set; }
        public DateTime Date { get; set; }
        public decimal Average { get; set; }
        public decimal Sum { get; set; }
        public int Count { get; set; }
    }
}
