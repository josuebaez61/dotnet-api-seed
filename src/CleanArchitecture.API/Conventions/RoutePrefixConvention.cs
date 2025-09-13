using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CleanArchitecture.API.Conventions
{
  public class RoutePrefixConvention : IControllerModelConvention
  {
    private readonly AttributeRouteModel _routePrefix;

    public RoutePrefixConvention(IRouteTemplateProvider route)
    {
      _routePrefix = new AttributeRouteModel(route);
    }

    public void Apply(ControllerModel controller)
    {
      foreach (var selector in controller.Selectors)
      {
        if (selector.AttributeRouteModel != null)
        {
          selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
              _routePrefix, selector.AttributeRouteModel);
        }
        else
        {
          selector.AttributeRouteModel = _routePrefix;
        }
      }
    }
  }
}
