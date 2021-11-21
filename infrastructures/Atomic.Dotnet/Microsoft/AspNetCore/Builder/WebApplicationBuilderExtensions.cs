using System.Reflection;
using Atomic.AspNetCore.Mvc.Conventions;
using Microsoft.AspNetCore.Mvc;
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
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFileName = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
        });
    }
}