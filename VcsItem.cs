using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VcfToSimplyNote
{
    public class VcsItem
    {
        public string Summary { set; get; }
        public string Description { set; get; }
        public DateTime Modified { set; get; }
        public DateTime Created { set; get; }
    }
}
