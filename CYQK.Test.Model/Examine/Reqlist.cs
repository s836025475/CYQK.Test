using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CYQK.Test.Model.Examine
{
    [Table("t_reqlist")]
    public class Reqlist
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public virtual long Id { get; set; }
        /// <summary>
        /// 审核类型
        /// </summary>
        public virtual string Fbilltype { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        public virtual string Fbillno { get; set; }
        /// <summary>
        /// 单据Id
        /// </summary>
        public virtual string Fbillid { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public virtual string Fcheckerman { get; set; }
        /// <summary>
        /// 审核级次
        /// </summary>
        public virtual string Fcheckstep { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreateTime { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public virtual DateTime? HandleTime { get; set; }
    }
}
