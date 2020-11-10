using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CYQK.Test.Model.Examine
{
    [Table("t_CGSqlist")]
    public class CGSqlist
    {
        [Key]
        public virtual long Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// FormInstId
        /// </summary>
        public virtual string FormInstId { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public virtual string SerialNumber { get; set; }
        /// <summary>
        /// FormCodeId
        /// </summary>
        public virtual string FormCodeId { get; set; }
        /// <summary>
        /// Fbillid
        /// </summary>
        public virtual string Fbillid { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [Column(TypeName ="decimal(18,6)")]
        public virtual decimal Freqamount { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        public virtual string Fuseman { get; set; }
        /// <summary>
        /// 市场区域
        /// </summary>
        public virtual string FmarkertOrea { get; set; }
        /// <summary>
        /// 提交人
        /// </summary>
        public virtual string Fsubmitman { get; set; }
        /// <summary>
        /// 申请内容
        /// </summary>
        public virtual string FrequestContext { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public virtual string Fdepartment { get; set; }
        /// <summary>
        /// 第一次输入
        /// </summary>
        public virtual bool FirstInput { get; set; }
        /// <summary>
        /// 费用类型
        /// </summary>
        public virtual string FeeType { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        [Column(TypeName = "decimal(18,6)")]
        public virtual decimal TotalFee { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public virtual long TotalCount { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public virtual DateTime EventTime { get; set; }
    }
}
