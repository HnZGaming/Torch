using System.IO;
using System.Xml.Serialization;
using NLog;

namespace Torch.Server.ElasticSearch
{
    public class ElasticConfig
    {
        const string ConfigName = "Elastic.cfg";
        const string DefaultNodeUrl = "http://localhost:9200";

        static readonly Logger _logger = LogManager.GetLogger(nameof(ElasticManager));

        /// <summary>
        /// URL of the elasticsearch node.
        /// Example: http://localhost:9200
        /// </summary>
        [XmlElement]
        public string NodeUrl { get; set; }

        /// <summary>
        /// Load the config file from the disk.
        /// </summary>
        /// <remarks>
        /// New config file will be created if not found.
        /// </remarks>
        /// <returns>Loaded or created config.</returns>
        public static ElasticConfig Load()
        {
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigName);

            var configSerializer = new XmlSerializer(typeof(ElasticConfig));

            if (!File.Exists(ConfigName))
            {
                _logger.Info($"Generating default config at {configPath}");

                var config = new ElasticConfig
                {
                    NodeUrl = DefaultNodeUrl,
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
                return (ElasticConfig) configSerializer.Deserialize(file);
            }
        }
    }
}