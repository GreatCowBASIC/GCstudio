using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC_Studio
{
    public class Module
    {
        public string Name { get; set; } = null;
        public bool Enabled { get; set; } = true;
        public bool Deployed { get; set; } = false;
    }
}

