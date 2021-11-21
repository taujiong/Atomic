using System.Reflection;
using Atomic.AspNetCore.Mvc.ApiExplorer;
using Atomic.AspNetCore.Mvc.Conventions;
using Atomic.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureApiController(this WebApplicationBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        var routeArea = builder.Configuration["App:RouteArea"]
                        ?? builder.Configuration["App:AppName"]?.ToKebabCase()
                        ?? "app";

        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Conventions.Add(new AtomicApiControllerConvention(routeArea));
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddTransient<IApiDescriptionProvider, ResponseTypeApiDescriptionProvider>();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFileName = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
        });
    }

    public static void AddAtomicLocalization(this WebApplicationBuilder builder, IMvcBuilder mvcBuilder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.Services.AddLocalization();
        mvcBuilder.AddDataAnnotationsLocalization(options =>
        {
            options.DataAnnotationLocalizerProvider = (_, factory) =>
                factory.Create(typeof(AtomicSharedResource));
        });
    }
}