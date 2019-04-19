using System;

namespace UI.TestUtils
{
    public static class Constants
    {
        public static class Paths
        {
            public static readonly string MewMainPath = @"c:\Program Files (x86)\BTL\MEW\MEW.Main.exe";
            public static readonly string MewHolterPath = @"c:\Program Files (x86)\BTL\MEW\MEW.Holter2g.exe";
            public static readonly string MewAgentPath = @"c:\Program Files (x86)\BTL\MEW\MEW.Agent.exe";
            public static readonly string MewReportViewerPath = @"c:\Program Files (x86)\BTL\MEW\PdfWebViewer.exe";
            public static readonly string MewMainPath3 = @"c:\CardioPoint3\CP3\CP.Client.Main.exe";
        }

        public static class Times
        {
            public static readonly TimeSpan DefaultTimeSpan = TimeSpan.FromSeconds(30);
            public static readonly TimeSpan InstallationTimeSpan = TimeSpan.FromSeconds(120);
            public static readonly TimeSpan NetConsultTimeSpan = TimeSpan.FromSeconds(90);
        }
        public static class Names
        {
            public static readonly string UI = "UI";
            public static readonly string AD = "_AD";
            public static readonly string CPU = "_CPU";
            public static readonly string SSO = "_SSO";
        }

        public static readonly bool MouseMove = true;
        public static readonly int Iterations = 30;
    }
}
