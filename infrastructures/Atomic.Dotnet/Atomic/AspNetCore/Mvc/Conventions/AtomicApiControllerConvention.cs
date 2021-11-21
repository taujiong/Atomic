using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Atomic.AspNetCore.Mvc.Conventions;

public class AtomicApiControllerConvention : IApplicationModelConvention
{
    private readonly string _routeArea;

    public AtomicApiControllerConvention(string routeArea)
    {
        _routeArea = routeArea;
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    var kebabControllerName = controller.ControllerName.ToKebabCase();
                    selector.AttributeRouteModel.Template = $"api/{_routeArea}/{kebabControllerName}s";
                }
            }
        }
    }
}