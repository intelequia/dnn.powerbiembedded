using Newtonsoft.Json;

namespace DotNetNuke.PowerBI.Services.Models
{
    public class AzureCapacity
    {
        public string DisplayName { get; set; }

        public string Sku { get; set; }

        public string State { get; set; }

        public string Region { get; set; }

        public AzureCapacity() { }
    }
}