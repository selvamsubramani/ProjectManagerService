using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using System.Web.Http.Cors;
namespace ProjectManagement.Service
{
    [ExcludeFromCodeCoverage]
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ProjectManagementApi",
                routeTemplate: "ProjectManagementService/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Enable CORS
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
        }
    }
}
