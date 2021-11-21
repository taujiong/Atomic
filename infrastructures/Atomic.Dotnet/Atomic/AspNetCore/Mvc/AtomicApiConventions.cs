using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Atomic.AspNetCore.Mvc;

public static class AtomicApiConventions
{
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Get(
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix),
         ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
        object id
    )
    {
    }

    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void GetList(
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any),
         ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
        object model
    )
    {
    }

    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Create(
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any),
         ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
        object model
    )
    {
    }

    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(404)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Update(
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix),
         ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
        object id,
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any),
         ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
        object model
    )
    {
    }

    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Delete(
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix),
         ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
        object id
    )
    {
    }
}