using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Nest;
using NLog;
using Torch.API;
using Torch.Managers;
using Torch.Server.Annotations;

namespace Torch.Server.ElasticSearch
{
    /// <summary>
    /// Provide the singleton ElasticClient instance.
    /// </summary>
    public sealed class ElasticManager : Manager
    {
        const string ConfigName = "Elastic.cfg";
        const string DefaultNodeUrl = "http://localhost:9200";

        static readonly Logger _logger = LogManager.GetLogger(nameof(ElasticManager));
        readonly CancellationTokenSource _canceller;

        /// <inheritdoc/>
        public ElasticManager(ITorchBase torch) : base(torch)
        {
            _canceller = new CancellationTokenSource();
        }

        /// <summary>
        /// Singleton ElasticClient instance.
        /// </summary>
        [UsedImplicitly]
        public ElasticClient ElasticClient { get; private set; }

        /// <inheritdoc/>
        public override void Attach()
        {
            try
            {
                var config = InitConfig();
                var settings = new ConnectionSettings(new Uri(config.NodeUrl));
                ElasticClient = new ElasticClient(settings);

                // ping the elasticsearch server & output error log if 404
                Task.Factory.StartNew(PingTest, _canceller.Token);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        /// <inheritdoc/>
        public override void Detach()
        {
            ElasticClient = null;
        }

        static ElasticConfig InitConfig()
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

        void PingTest()
        {
            var response = ElasticClient.Ping();
            if (response.IsValid)
            {
                _logger.Info("Elasticsearch server responded");
            }
            else
            {
                _logger.Warn("Elasticsearch server not responding");
            }
        }
    }
}