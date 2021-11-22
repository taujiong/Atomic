using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static void UseAtomicControllerPreset(this WebApplication app)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }

    public static void UseAtomicLocalization(this WebApplication app)
    {
        var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(localizationOptions.Value);
    }
}