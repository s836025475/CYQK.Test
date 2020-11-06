using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CYQK.Test.Dto
{
    public class ReturnMessage
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public object Data { get; set; }
    }
}
