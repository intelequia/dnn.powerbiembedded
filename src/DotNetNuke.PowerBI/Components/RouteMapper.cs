using DotNetNuke.Web.Api;

namespace DotNetNuke.PowerBI.Components
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute routeManager)
        {
            routeManager.MapHttpRoute("PowerBI/UI", "default", "{controller}/{action}", new[] { "DotNetNuke.PowerBI.Controllers.Api.Admin" });
            routeManager.MapHttpRoute("PowerBI/Services", "default", "{controller}/{action}", new[] { "DotNetNuke.PowerBI.Services" });
        }
    }
}