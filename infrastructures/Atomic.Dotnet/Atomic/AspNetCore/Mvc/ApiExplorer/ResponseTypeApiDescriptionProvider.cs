using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Atomic.AspNetCore.Mvc.ApiExplorer;

public class ResponseTypeApiDescriptionProvider : IApiDescriptionProvider
{
    private readonly IModelMetadataProvider _modelMetadataProvider;
    private readonly MvcOptions _mvcOptions;

    public ResponseTypeApiDescriptionProvider(
        IModelMetadataProvider modelMetadataProvider,
        IOptions<MvcOptions> mvcOptions
    )
    {
        _modelMetadataProvider = modelMetadataProvider;
        _mvcOptions = mvcOptions.Value;
    }

    // The order of DefaultApiDescriptionProvider is -1000.
    // -999 ensures that this one executes right after the default one.
    public int Order => -999;

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        foreach (var apiDescription in context.Results)
        {
            var hasAuthorizeAttribute =
                apiDescription.ActionDescriptor.EndpointMetadata.Any(meta =>
                    meta.GetType() == typeof(AuthorizeAttribute));
            if (hasAuthorizeAttribute)
            {
                apiDescription.SupportedResponseTypes.Add(CreateResponseType(StatusCodes.Status401Unauthorized));
                apiDescription.SupportedResponseTypes.Add(CreateResponseType(StatusCodes.Status403Forbidden));
            }

            apiDescription.SupportedResponseTypes.Add(CreateResponseType(StatusCodes.Status500InternalServerError));
        }
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
    }

    private ApiResponseType CreateResponseType(int statusCode)
    {
        var responseType = new ApiResponseType
        {
            StatusCode = statusCode,
            Type = typeof(ProblemDetails),
        };
        responseType.ModelMetadata = _modelMetadataProvider.GetMetadataForType(responseType.Type);

        foreach (var responseTypeMetadataProvider in _mvcOptions.OutputFormatters
                     .OfType<IApiResponseTypeMetadataProvider>())
        {
            var formatterSupportedContentTypes =
                responseTypeMetadataProvider.GetSupportedContentTypes(null!, responseType.Type);
            if (formatterSupportedContentTypes == null)
            {
                continue;
            }

            foreach (var formatterSupportedContentType in formatterSupportedContentTypes)
            {
                responseType.ApiResponseFormats.Add(new ApiResponseFormat
                {
                    Formatter = (IOutputFormatter)responseTypeMetadataProvider,
                    MediaType = formatterSupportedContentType,
                });
            }
        }

        return responseType;
    }
}