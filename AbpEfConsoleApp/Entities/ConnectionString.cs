using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpEfConsoleApp.Entities
{
    public class ConnectionString : FullAuditedEntity<long>
    {
        [Required]
        [StringLength(AnalyticsConsts.MaxLengths.DatabaseName)]
        public string DatabaseName { get; set; }

        [StringLength(AnalyticsConsts.MaxLengths.ConnectionString)]
        public string ConnectionStringValue { get; set; }
    }
}
