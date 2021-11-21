using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.ExceptionHandling;

public class EntityNotFoundException : AtomicException
{
    public EntityNotFoundException(
        Type entityType,
        object value,
        string key = "id",
        Exception? innerException = null
    ) : base(
        "There is no such an entity with given {0}={1}.",
        "Please check your input.",
        innerException)
    {
        EntityType = entityType;
        Value = value;
        Key = key;
    }

    public Type EntityType { get; }

    public object Value { get; }

    public string Key { get; }

    public override object[] MessageLocalizationParameters => new[] { Key, Value };

    public override ProblemDetails ToProblemDetails()
    {
        var details = base.ToProblemDetails();
        details.Extensions["entityType"] = EntityType.FullName;
        details.Extensions[Key] = Value;
        details.Status = StatusCodes.Status404NotFound;

        return details;
    }
}