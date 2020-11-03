using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CYQK.Test.Model
{
    public class TestEntity
    {
        [Key]
        public virtual long Id { get; set; }
        
        public virtual string Name { get; set; }
    }
}
