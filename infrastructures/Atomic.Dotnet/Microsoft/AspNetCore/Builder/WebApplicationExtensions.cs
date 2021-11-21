using Microsoft.Extensions.Hosting;

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
}