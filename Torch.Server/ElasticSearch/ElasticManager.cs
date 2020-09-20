using System;
using System.Threading;
using System.Threading.Tasks;
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
        static readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
                var config = ElasticConfig.Load();
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