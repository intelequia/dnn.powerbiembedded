using System.Web;

namespace DotNetNuke.PowerBI.Extensibility
{
    public interface IRlsCustomExtension
    {
        string GetRlsValue(HttpContext httpContext);
    }
}
