using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CYQK.Test.Model.Examine
{
    [Table("t_CgsqListentry")]
    public class CgsqListentry
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid Guid { get; set; }
        /// <summary>
        /// Fbillid
        /// </summary>
        public virtual string Fbillid { get; set; }
        /// <summary>
        /// 用酒名称
        /// </summary>
        public virtual string WineName { get; set; }
        /// <summary>
        /// 用酒数量
        /// </summary>
        public virtual int WineCount { get; set; }
        /// <summary>
        /// 用酒费用
        /// </summary>
        [Column(TypeName = "decimal(18,6)")]
        public virtual decimal WineFee { get; set; }
    }
}
