using DotNetNuke.Web.Api;

namespace DotNetNuke.PowerBI.Components
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute routeManager)
        {
            //routeManager.MapHttpRoute("PowerBI", "default", "{controller}/{action}", new[] { "DotNetNuke.PowerBI.Controllers" });
            routeManager.MapHttpRoute("PowerBI/UI", "default", "{controller}/{action}", new[] { "DotNetNuke.PowerBI.Controllers.Api.Admin" });
            routeManager.MapHttpRoute("Bookmarks", "default", "{controller}/{action}", new[] { "DotNetNuke.PowerBI.Services" });
            routeManager.MapHttpRoute("Subscription", "default", "{controller}/{action}", new[] { "DotNetNuke.PowerBI.Services" });
        }

    }
}