using System;

namespace GC_Studio
{
    public class ConfigSchema
    {

        public GCstudioConfig GCstudio = new GCstudioConfig();
        public WindowConfig Window = new WindowConfig(); 

        public class WindowConfig
        {

            public int sizeW { get; set; }
            public int sizeH { get; set; }
            public Int32 locx { get; set; }
            public Int32 locy { get; set; }
            public bool maximized { get; set; }
        }

        public class GCstudioConfig
        {
            public string ReleaseChanel { get; set; }
            public string IDE { get; set; }
            public string Architecture { get; set; }
            public bool Firstrun { get; set; }
            public string LastDirectory { get; set; }
        }

    }
}
