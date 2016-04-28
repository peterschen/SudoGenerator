using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace SudoGenerator.Models
{
    public class Configuration
    {
        private const string PATH_CONFIG = "conf";

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

            configuration.Append(GenerateConfigurationGeneral());

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

            configuration.AppendLine("\n# End user configuration for Operations Manager agent");
            configuration.AppendLine("# -----------------------------------------------------------------------------------");

            return configuration.ToString();
        }

        private string GenerateConfigurationGeneral()
        {
            return ParseConfiguration("general", new Dictionary<string, string> { { "MonitoringUser", MonitoringUser } });
        }

        private string GenerateConfigurationMaintenance()
        {
            return ParseConfiguration("maintenance", new Dictionary<string, string> { { "MonitoringUser", MonitoringUser } });
        }

        private string GenerateConfigurationLogs()
        {
            return ParseConfiguration("logs", new Dictionary<string, string> { { "MonitoringUser", MonitoringUser } });
        }

        private string GenerateConfigurationSamples()
        {
            return ParseConfiguration("samples", new Dictionary<string, string> { { "MonitoringUser", MonitoringUser } });
        }

        private string ParseConfiguration(string subject, Dictionary<string, string> replacements = null)
        {
            string partialConfiguration = ReadConfiguration(subject);

            if(replacements != null)
            {
                foreach(KeyValuePair<string, string> replacement in replacements)
                {
                    partialConfiguration = partialConfiguration.Replace("{" + replacement.Key + "}", replacement.Value);
                }
            }

            return partialConfiguration;
        }

        private string ReadConfiguration(string subject)
        {
            string path = string.Format("{0}/{1}/{2}.{3}.txt", PATH_CONFIG, ProductVersion, OperatingSystem, subject);
            string partialConfig = string.Empty;

            if (File.Exists(path))
            {
                partialConfig = File.ReadAllText(path);
            }

            return partialConfig;
        }
    }
}