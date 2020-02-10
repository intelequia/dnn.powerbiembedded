using System;

namespace DotNetNuke.PowerBI.Models
{
    [Serializable]
    public class TileEmbedConfig : EmbedConfig
    {
        public string dashboardId { get; set; }
    }
}