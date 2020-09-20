using System.IO;
using System.Xml.Serialization;
using NLog;

namespace Torch.Server.Firebase
{
    /// <summary>
    /// Config for Firebase integration.
    /// </summary>
    public class FirebaseConfig
    {
        const string ConfigName = "Firebase.cfg";

        static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Path to the Google Credential JSON file.
        /// </summary>
        [XmlElement]
        public string GoogleCredentialJsonPath { get; set; }

        /// <summary>
        /// Load the config file from the disk.
        /// </summary>
        /// <remarks>
        /// New config file will be created if not found.
        /// </remarks>
        /// <returns>Loaded or created config.</returns>
        public static FirebaseConfig Load()
        {
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigName);

            var configSerializer = new XmlSerializer(typeof(FirebaseConfig));

            if (!File.Exists(ConfigName))
            {
                _logger.Info($"Generating default config at {configPath}");

                var config = new FirebaseConfig
                {
                    GoogleCredentialJsonPath = "GoogleCredential.json",
                };

                using (var file = File.Create(configPath))
                {
                    configSerializer.Serialize(file, config);
                }

                return config;
            }

            _logger.Info($"Loading config {configPath}");

            using (var file = File.OpenRead(configPath))
            {
                return (FirebaseConfig) configSerializer.Deserialize(file);
            }
        }
    }
}