using System.Collections.Generic;

namespace SudoGenerator.Classes
{
    public class OperatingSystem
    {
        public string Id { get; set; }
        public string Value { get; set; }

        public static readonly OperatingSystem AIX = new OperatingSystem("aix", "AIX");
        public static readonly OperatingSystem HPUX = new OperatingSystem("hpux", "HP-UX");
        public static readonly OperatingSystem RHEL = new OperatingSystem("rhel", "Red Hat Enterprise Linux");
        public static readonly OperatingSystem SLES = new OperatingSystem("sles", "SuSE Linux Enterprise Server");
        public static readonly OperatingSystem SOLARIS = new OperatingSystem("solaris", "Solaris");
        public static readonly OperatingSystem DEB = new OperatingSystem("universal-deb", "Universal .deb (Debian, Ubuntu)");
        public static readonly OperatingSystem RPM = new OperatingSystem("universal-rpm", "Universal .rpm (CentOS, Oracle Linux)");
        public static readonly OperatingSystem LINUX = new OperatingSystem("linux", "Linux (Red Hat Enterprise Linux / SuSE Linux Enterprise");

        public static readonly List<OperatingSystem> SC2012RTM = new List<OperatingSystem> { AIX, HPUX, LINUX, SOLARIS };
        public static readonly List<OperatingSystem> SC2012R2 = new List<OperatingSystem> { AIX, HPUX, RHEL, SLES, SOLARIS, DEB, RPM };
        public static readonly List<OperatingSystem> SC2016RTM = new List<OperatingSystem> { AIX, HPUX, RHEL, SLES, SOLARIS, DEB, RPM };

        public OperatingSystem(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}