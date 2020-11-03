using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CYQK.Test.Model
{
    public class DefaultTable
    {
        [Key]
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool Sex { get; set; }
    }
}
