using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CYQK.Test.Model.Examine
{
    public class ExternalLog
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public virtual long Id { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public virtual DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public virtual DateTime EndTime { get; set; }
        /// <summary>
        /// 查询转态
        /// </summary>
        public virtual QueryState QueryState { get; set; }
        /// <summary>
        /// 未传数据总条数
        /// </summary>
        public virtual int FailTotalNum { get; set; }
        /// <summary>
        /// 未传数据抓取状态
        /// </summary>
        public virtual TakeStatus TakeStatus { get; set; }
        /// <summary>
        /// 未传数据抓取行数
        /// </summary>
        public virtual int CatchNum { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreationTime { get; set; }
        /// <summary>
        /// 最新抓取时间
        /// </summary>
        public virtual DateTime TakeTime { get; set; }
    }
    public enum QueryState
    { 
        失败 = 0,
        成功 = 1
    }
    public enum TakeStatus
    { 
        未同步 = 0,
        部分同步 = 1,
        完全同步 = 2
    }
}
