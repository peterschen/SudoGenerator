using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SudoGenerator.Models
{
    public class Configuration
    {
        public string ProductVersion { get; set;}
        public string OperatingSystem { get; set; }

        [Required]
        public string MonitoringUser { get; set; }
        public bool IncludeMaintenance { get; set; }
        public bool IncludeLogs { get; set; }
        public bool IncludeSamples { get; set; }

        public bool HasConfiguration { get; private set; }

        public void Generate()
        {
            if(ProductVersion != null && OperatingSystem != null && !string.IsNullOrEmpty(MonitoringUser))
            {
                HasConfiguration = true;
            }
        }

        public string GetConfiguration()
        {
            var configuration = new StringBuilder();

            configuration.AppendLine("# -----------------------------------------------------------------------------------");
            configuration.AppendFormat("# User configuration for Operations Manager agent – for a user with the name: {0}\n\n", MonitoringUser);

            if (OperatingSystem != "hpux")
            {
                configuration.AppendLine("# General requirements");
                if (OperatingSystem == "aix" || OperatingSystem == "solaris")
                {
                    configuration.AppendFormat("Defaults:{0} passwd_tries = 1, passwd_timeout = 1\n\n", MonitoringUser);
                }
                else
                {
                    configuration.AppendFormat("Defaults:{0} !requiretty\n\n", MonitoringUser);
                }
            }

            if (IncludeMaintenance)
            {
                configuration.Append(GenerateConfigurationMaintenance());
                configuration.AppendLine();
            }

            if (IncludeLogs)
            {
                configuration.Append(GenerateConfigurationLogs());
                configuration.AppendLine();
            }

            if (IncludeSamples)
            { 
                configuration.Append(GenerateConfigurationSamples());
                configuration.AppendLine();
            }

            configuration.AppendLine("# End user configuration for Operations Manager agent");
            configuration.AppendLine("# -----------------------------------------------------------------------------------");

            return configuration.ToString();
        }

        private string GenerateConfigurationMaintenance()
        {
            StringBuilder partialConfig = new StringBuilder();

            partialConfig.AppendLine("# Agent maintenance (discovery, install, uninstall, upgrade, restart, certificate signing");

            if (OperatingSystem == "aix")
            {
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c cp /tmp/scx-{0}/scx.pem /etc/opt/microsoft/scx/ssl/scx.pem; rm -rf /tmp/scx-{0}; /opt/microsoft/scx/bin/tools/scxadmin -restart\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c sh /tmp/scx-{0}/GetOSVersion.sh; EC=$?; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c cat /etc/opt/microsoft/scx/ssl/scx.pem\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c /usr/sbin/installp -u scx\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c gzip -dqf /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].aix.[0-9].ppc.lpp.gz;/usr/sbin/installp -a -d /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].aix.[0-9].ppc.lpp scx; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
            }
            else if(OperatingSystem == "hpux")
            {
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c sh /tmp/scx-{0}/GetOSVersion.sh; EC=$?; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);

                partialConfig.AppendLine("\n## Intel Itanium (IA-64)");
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c uncompress -f /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].hpux.11iv[0-9].ia64.depot.Z;/usr/sbin/swinstall -s /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].hpux.11iv[0-9].ia64.depot scx; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);

                partialConfig.AppendLine("\n## PA-RISC");
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c uncompress -f /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].hpux.11iv[0-9].parisc.depot.Z;/usr/sbin/swinstall -s /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].hpux.11iv[0-9].parisc.depot scx; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);

                partialConfig.AppendFormat("\n{0} ALL=(root) NOPASSWD: /usr/bin/sh -c cat /etc/opt/microsoft/scx/ssl/scx.pem\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c cp /tmp/scx-{0}/scx.pem /etc/opt/microsoft/scx/ssl/scx.pem; rm -rf /tmp/scx-{0}; /opt/microsoft/scx/bin/tools/scxadmin -restart\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c /usr/sbin/swremove scx\n", MonitoringUser);
            }
            else if (OperatingSystem == "rhel")
            {
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c cp /tmp/scx-{0}/scx.pem /etc/opt/microsoft/scx/ssl/scx.pem; rm -rf /tmp/scx-{0}; /opt/microsoft/scx/bin/tools/scxadmin -restart\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c sh /tmp/scx-{0}/GetOSVersion.sh; EC=$?; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c  cat /etc/opt/microsoft/scx/ssl/scx.pem\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c  rpm -e scx\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c /bin/rpm -F --force /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].rhel.[0-9].x[6-8][4-6].rpm; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c /bin/rpm -U --force /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].rhel.[0-9].x[6-8][4-6].rpm; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
            }
            else if (OperatingSystem == "sles")
            {
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c cp /tmp/scx-{0}/scx.pem /etc/opt/microsoft/scx/ssl/scx.pem; rm -rf /tmp/scx-{0}; /opt/microsoft/scx/bin/tools/scxadmin -restart\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c sh /tmp/scx-{0}/GetOSVersion.sh; EC=$?; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c cat /etc/opt/microsoft/scx/ssl/scx.pem\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c  rpm -e scx\n", MonitoringUser);


                partialConfig.AppendLine("\n## SuSE Linux Enterprise Server 9");
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c /bin/rpm -F --force /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].sles.9.x[6-8][4-6].rpm; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c /bin/rpm -U --force /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].sles.9.x[6-8][4-6].rpm; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);

                partialConfig.AppendLine("\n## SuSE Linux Enterprise Server 10/11/12");
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c /bin/rpm -F --force /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].sles.1[0|1|2].x[6-8][4-6].rpm; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c /bin/rpm -U --force /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].sles.1[0|1|2].x[6-8][4-6].rpm; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
            }
            else if (OperatingSystem == "universal-deb")
            {
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c cp /tmp/scx-{0}/scx.pem /etc/opt/microsoft/scx/ssl/scx.pem; rm -rf /tmp/scx-{0}; /opt/microsoft/scx/bin/tools/scxadmin -restart\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c sh /tmp/scx-{0}/GetOSVersion.sh; EC=$?; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c cat /etc/opt/microsoft/scx/ssl/scx.pem\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c dpkg -P scx\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c dpkg -i /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].universald.1.x[6-8][4-6].deb; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
            }
            else if (OperatingSystem == "universal-rpm")
            {
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c cp /tmp/scx-{0}/scx.pem /etc/opt/microsoft/scx/ssl/scx.pem; rm -rf /tmp/scx-{0}; /opt/microsoft/scx/bin/tools/scxadmin -restart\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c sh /tmp/scx-{0}/GetOSVersion.sh; EC=$?; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c cat /etc/opt/microsoft/scx/ssl/scx.pem\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c rpm -e scx\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c /bin/rpm -F --force /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].universalr.[0-9].x[6-8][4-6].rpm; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /bin/sh -c /bin/rpm -U --force /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].universalr.[0-9].x[6-8][4-6].rpm; EC=$?; cd /tmp; rm -rf /tmp/scx-{0}; exit $EC\n", MonitoringUser);
            }
            else if (OperatingSystem == "solaris")
            {
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c sh /tmp/scx-{0}/GetOSVersion.sh; EC=??; rm -rf /tmp/scx-{0}; exit ?EC\n", MonitoringUser);

                partialConfig.AppendLine("\n## Solaris 9");
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c echo -e \"mail=*/usr/sbin/pkgadd -a /tmp/scx-{0}/scx -n -d /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].solaris.9.sparc.pkg MSFTscx;*exit ?EC\n", MonitoringUser);

                partialConfig.AppendLine("\n## Solaris 10/11");
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c echo -e \"mail=*/usr/sbin/pkgadd -a /tmp/scx-{0}/scx -n -d /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].solaris.1[0-1].sparc.pkg MSFTscx;*exit ?EC\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c echo -e \"mail=*/usr/sbin/pkgadd -a /tmp/scx-{0}/scx -n -d /tmp/scx-{0}/scx-1.[0-9].[0-9]-[0-9][0-9][0-9].solaris.1[0-1].x86.pkg MSFTscx;*exit ?EC\n", MonitoringUser);

                partialConfig.AppendFormat("\n{0} ALL=(root) NOPASSWD: /usr/bin/sh -c rm -rf /tmp/scx-{0};*/usr/sbin/pkgrm -a /tmp/scx-{0}/scx -n MSFTscx;*exit ?EC\n", MonitoringUser);

                partialConfig.AppendFormat("\n{0} ALL=(root) NOPASSWD: /usr/bin/sh -c cat /etc/opt/microsoft/scx/ssl/scx.pem\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c rm -rf /tmp/scx-{0}\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /usr/bin/sh -c cp /tmp/scx-{0}/scx.pem /etc/opt/microsoft/scx/ssl/scx.pem; rm -rf /tmp/scx-{0}; /opt/microsoft/scx/bin/tools/scxadmin -restart\n", MonitoringUser);
                partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /opt/microsoft/scx/bin/tools/scxadmin\n", MonitoringUser);
            }

            return partialConfig.ToString();
        }

        private string GenerateConfigurationLogs()
        {
            StringBuilder partialConfig = new StringBuilder();

            partialConfig.AppendLine("# Log file monitoring");
            partialConfig.AppendFormat("{0} ALL=(root) NOPASSWD: /opt/microsoft/scx/bin/scxlogfilereader -p\n", MonitoringUser);

            return partialConfig.ToString();
        }

        private string GenerateConfigurationSamples()
        {
            StringBuilder partialConfig = new StringBuilder();

            partialConfig.Append("# Samples\n");

            partialConfig.Append("## Custom shell command monitoring example – replace <shell command> with the correct command string\n");
            partialConfig.AppendFormat("#{0} ALL=(root) NOPASSWD: /bin/bash -c <shell command>\n", MonitoringUser);

            partialConfig.Append("\n## Daemon diagnostic and restart recovery tasks example (using cron)\n");
            partialConfig.AppendFormat("#{0} ALL=(root) NOPASSWD: /bin/sh -c ps -ef | grep cron | grep -v grep\n", MonitoringUser);
            partialConfig.AppendFormat("#{0} ALL=(root) NOPASSWD: /usr/sbin/cron &\n", MonitoringUser);

            return partialConfig.ToString();
        }
    }
}