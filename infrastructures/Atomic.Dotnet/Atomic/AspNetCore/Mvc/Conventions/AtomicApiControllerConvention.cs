using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

            // Complex model in ApiController will use FromBodyAttribute
            // while http get method can not get data from http request body
            // so change all FromBodyAttribute to FromQueryAttribute
            var httpGetMethods = controller.Actions.Where(
                action => action.Attributes.Any(attribute => attribute is HttpGetAttribute)
            );
            foreach (var httpGetMethod in httpGetMethods)
            {
                foreach (var parameter in httpGetMethod.Parameters)
                {
                    if (parameter.BindingInfo?.BindingSource?.Id == "Body")
                    {
                        parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromQueryAttribute() });
                    }
                }
            }
        }
    }
}