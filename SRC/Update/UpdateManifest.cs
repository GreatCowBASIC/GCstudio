using System;


namespace Update
{
    public class UpdateManifest
    {
        public UpdateManifestInfo UpdateInfo = new UpdateManifestInfo();

        public class UpdateManifestInfo
        {
            public UpdateManifestInfo()
            {
                ManifestVer = 0;
                ManifestMinVer = 0;
                ManifestPKG = null;
                ManifestChecksum = null;
                ManifestTitle = null;
                ManifestNotes = null;
                AppExe = null;
            }
            public double ManifestVer { get; set; }
            public double ManifestMinVer { get; set; }
            public string ManifestPKG { get; set; }
            public string ManifestChecksum { get; set; }
            public string ManifestTitle { get; set; }
            public string ManifestNotes { get; set; }
            public string AppExe { get; set; }
        }

    }
}
