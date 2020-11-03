using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CYQK.Test.Model
{
    public class TestLog
    {
        [Key]
        public Guid Id { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
