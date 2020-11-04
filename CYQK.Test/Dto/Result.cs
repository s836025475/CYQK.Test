using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CYQK.Test.Dto
{
    public class Result
    {
        public bool success { get; set; }
        public string error { get; set; }
        public Data data { get; set; }
    }
}
