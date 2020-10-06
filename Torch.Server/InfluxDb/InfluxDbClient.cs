using System;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using NLog;

namespace Torch.Server.InfluxDb
{
    /// <summary>
    /// Wrap the endpoint of the InfluxDB instance.
    /// </summary>
    public class InfluxDbClient : IDisposable
    {
        static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        readonly InfluxDbConfig _config;
        readonly InfluxDBClient _influxClient;
        readonly WriteApi _writeApi;

        /// <summary>
        /// Instantiate class.
        /// </summary>
        /// <param name="config">Config for this instance.</param>
        public InfluxDbClient(InfluxDbConfig config)
        {
            _config = config;

            _influxClient?.Dispose();
            _writeApi?.Dispose();

            _influxClient = InfluxDBClientFactory.Create(
                config.DbHost,
                config.Token.ToCharArray());

            _writeApi = _influxClient.GetWriteApi();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _influxClient.Dispose();
            _writeApi.Dispose();
        }

        /// <summary>
        /// Instantiate a new PointData object with some properties set.
        /// </summary>
        /// <remarks>Timestamp will be set, but can be overwritten.</remarks>
        /// <param name="measurement"></param>
        /// <returns>New PointData object with some properties set.</returns>
        public PointData MakePointIn(string measurement)
        {
            // Note: UTC time is used
            var timestamp = DateTime.UtcNow;

            return PointData
                .Measurement(measurement)
                .Timestamp(timestamp, WritePrecision.S);
        }

        /// <summary>
        /// Write given points to the InfluxDB instance.
        /// </summary>
        /// <remarks>Exceptions or logs will not be thrown/recorded from this operation even if failed.</remarks>
        /// <param name="points">List of points to write to the DB instance.</param>
        public void WritePoints(params PointData[] points)
        {
            _writeApi.WritePoints(_config.Bucket, _config.Organization, points);
        }

        /// <summary>
        /// Write a message to the InfluxDB instance for checkpoint debugging.
        /// </summary>
        /// <param name="message">Message to send to the DB instance.</param>
        public void WritePing(string message)
        {
            var point = MakePointIn("ping")
                .Field("message", message);

            WritePoints(point);

            _logger.Info($"ping done: {message}");
        }
    }
}