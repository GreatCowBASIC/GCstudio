using System;

namespace GC_Studio
{
    public class ConfigSchema
    {

        public GCstudioConfig GCstudio = new GCstudioConfig();
        public WindowConfig Window = new WindowConfig();

        public class WindowConfig
        {
            public WindowConfig()
            {
                sizeW = 1028;
                sizeH = 681;
                locx = 0;
                locy = 0;
                maximized = false;
            }
            public int sizeW { get; set; }
            public int sizeH { get; set; }
            public Int32 locx { get; set; }
            public Int32 locy { get; set; }
            public bool maximized { get; set; }
        }

        public class GCstudioConfig
        {
            public GCstudioConfig()
            {
                ReleaseChanel = "mainstream";
                IDE = "GCcode";
                Architecture = "Auto";
                Firstrun = true;
                LastDirectory = null;
                Legacymode = false;
            }
            public string ReleaseChanel { get; set; }
            public string IDE { get; set; }
            public string Architecture { get; set; }
            public bool Firstrun { get; set; }
            public string LastDirectory { get; set; }
            public bool Legacymode { get; set; }
        }

    }
}
