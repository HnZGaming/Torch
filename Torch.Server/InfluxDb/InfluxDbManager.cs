using NLog;
using Torch.API;
using Torch.Managers;

namespace Torch.Server.InfluxDb
{
    /// <summary>
    /// Set up and manage lifecycle of the InfluxDB binding.
    /// </summary>
    public sealed class InfluxDbManager : Manager
    {
        const string ConfigName = "InfluxDbConfig.cfg";
        static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc/>
        public InfluxDbManager(ITorchBase torchInstance) : base(torchInstance)
        {
        }

        /// <summary>
        /// InfluxDB instance binder object.
        /// </summary>
        public InfluxDbClient Client { get; private set; }

        /// <inheritdoc/>
        public override void Attach()
        {
            var config = ConfigUtils.LoadConfigFile(ConfigName, InfluxDbConfig.Default);

            Client?.Dispose();
            Client = new InfluxDbClient(config);
        }

        /// <inheritdoc/>
        public override void Detach()
        {
            Client?.Dispose();
        }
    }
}