namespace Atomic.IdentityServer;

public record ClientSecretBaseDto
{
    public string? Description { get; set; }

    public DateTime Created { get; set; }
}

public record ClientSecretMaskedDto : ClientSecretBaseDto
{
    public int Id { get; set; }

    public string? MaskedValue { get; set; }
}

public record ClientSecretDto : ClientSecretBaseDto
{
    public string? Value { get; set; }
}