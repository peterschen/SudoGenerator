using System.IO;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.ApplicationInsights;

namespace SudoGenerator.Models
{
    public class Configuration
    {
        private static TelemetryClient telemetry = new TelemetryClient();

        private const string PATH_CONFIG = "Configurations";

        public string ProductVersion { get; set;}
        public string OperatingSystem { get; set; }

        [Required]
        public string MonitoringUser { get; set; }
        public bool IncludeMaintenance { get; set; }
        public bool IncludeLogs { get; set; }
        public bool IncludeSamples { get; set; }
        public bool IncludeOssManagement { get; set; }

        public bool HasConfiguration { get; private set; }

        public void Generate()
        {
            telemetry.TrackEvent("Configuration.Generate() called");

            if(ProductVersion != null && OperatingSystem != null && !string.IsNullOrEmpty(MonitoringUser))
            {
                HasConfiguration = true;
            }
        }

        public string GetConfiguration()
        {
            telemetry.TrackEvent("Configuration.GetConfiguration() called");
            
            var configuration = new StringBuilder();

            configuration.AppendLine("# -----------------------------------------------------------------------------------");
            configuration.AppendFormat("# User configuration for Operations Manager agent – for a user with the name: {0}\n\n", MonitoringUser);

            configuration.Append(GenerateConfigurationGeneral());
            configuration.AppendLine();

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

            if (IncludeOssManagement)
            {
                configuration.Append(GenerateCOnfigurationOssManagement());
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

        private string GenerateConfigurationGeneral()
        {
            telemetry.TrackEvent("Configuration.GenerateConfigurationGeneral() called");
            return ParseConfiguration("general", new Dictionary<string, string> { { "MonitoringUser", MonitoringUser } });
        }

        private string GenerateConfigurationMaintenance()
        {
            telemetry.TrackEvent("Configuration.GenerateConfigurationMaintenance() called");
            return ParseConfiguration("maintenance", new Dictionary<string, string> { { "MonitoringUser", MonitoringUser } });
        }

        private string GenerateConfigurationLogs()
        {
            telemetry.TrackEvent("Configuration.GenerateConfigurationLogs() called");
            return ParseConfiguration("logs", new Dictionary<string, string> { { "MonitoringUser", MonitoringUser } });
        }

        private string GenerateConfigurationSamples()
        {
            telemetry.TrackEvent("Configuration.GenerateConfigurationSamples() called");
            return ParseConfiguration("samples", new Dictionary<string, string> { { "MonitoringUser", MonitoringUser } });
        }

        private string GenerateCOnfigurationOssManagement()
        {
            telemetry.TrackEvent("Configuration.GenerateCOnfigurationOssManagement() called");
            return ParseConfiguration("ossmgmt", new Dictionary<string, string> { { "MonitoringUser", MonitoringUser } });
        }

        private string ParseConfiguration(string subject, Dictionary<string, string> replacements = null)
        {
            telemetry.TrackEvent("Configuration.ParseConfiguration() called");
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
            telemetry.TrackEvent("Configuration.ReadConfiguration() called");

            string path = string.Format("{0}/{1}/{2}.{3}.txt", PATH_CONFIG, ProductVersion, OperatingSystem, subject);
            string partialConfig = string.Empty;

            if (File.Exists(path))
            {
                partialConfig = File.ReadAllText(path);
            }
            else
            {
                throw new FileNotFoundException(path);
            }

            return partialConfig;
        }
    }
}