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
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public virtual long Id { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public virtual string Fbillid { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
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
        /// 申请内容
        /// </summary>
        public virtual string FrequestContext { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public virtual string Fdepartment { get; set; }
    }
}
