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
        /// Id
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// 费用类型
        /// </summary>
        public virtual string Fcosttype { get; set; }
        /// <summary>
        /// 申请金额
        /// </summary>
        public virtual string Fcostamount { get; set; }
    }
}
