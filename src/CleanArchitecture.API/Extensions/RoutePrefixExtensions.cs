using CleanArchitecture.API.Conventions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CleanArchitecture.API.Extensions
{
  public static class RoutePrefixExtensions
  {
    public static void UseGeneralRoutePrefix(this MvcOptions opts, string prefix)
    {
      opts.UseGeneralRoutePrefix(new RouteAttribute(prefix));
    }

    public static void UseGeneralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
    {
      opts.Conventions.Add(new RoutePrefixConvention(routeAttribute));
    }
  }
}
