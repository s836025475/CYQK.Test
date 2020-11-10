using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CYQK.Test.Dto.ExamineDto
{
    public class ApprovalProcess
    {
        /// <summary>
        /// 
        /// </summary>
        public string FlowInstId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Handler { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Imgs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AutographFileId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? HandleTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? CreateTime { get; set; }
        /// <summary>
        /// 开始审批
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 张三
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Files> Files { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActivityType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Opinion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
    }
}
