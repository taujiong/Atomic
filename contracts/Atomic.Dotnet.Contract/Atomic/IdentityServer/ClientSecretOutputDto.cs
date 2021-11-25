namespace Atomic.IdentityServer;

public record ClientSecretOutputDto
{
    public string? Description { get; set; }

    public string? MaskedValue { get; set; }

    public DateTime Created { get; set; }
}

public record ClientSecretDto
{
    public string? Description { get; set; }

    public string? Value { get; set; }
}