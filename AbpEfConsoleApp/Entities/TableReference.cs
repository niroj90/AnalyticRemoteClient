using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpEfConsoleApp.Entities
{
    public class TableReference:FullAuditedEntity<long>
    {
        public long ConnectionStringId { get; set; }
        [Required]
        [StringLength(AnalyticsConsts.MaxLengths.TableName)]
        public string TableName { get; set; }

        [Required]
        [StringLength(AnalyticsConsts.MaxLengths.ColumnName)]
        public string ColumnName { get; set; }

        [Required]
        [StringLength(AnalyticsConsts.MaxLengths.MappedColumnName)]
        public string MappedColumnName { get; set; }
    }
}
