using System.Xml.Serialization;

namespace Torch.Server.ElasticSearch
{
    public class ElasticConfig
    {
        /// <summary>
        /// URL of the elasticsearch node.
        /// Example: http://localhost:9200
        /// </summary>
        [XmlElement]
        public string NodeUrl { get; set; }
    }
}