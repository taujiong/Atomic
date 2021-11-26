using System.ComponentModel.DataAnnotations;

namespace Atomic.IdentityServer;

public record ClientListOutputDto
{
    public string? ClientId { get; set; }

    [Required]
    public string? ClientName { get; set; }

    public string? Description { get; set; }

    public string? LogoUri { get; set; }
}

public record ClientOutputDto : ClientListOutputDto
{
    public string? ClientUri { get; set; }

    public bool AllowAccessTokensViaBrowser { get; set; }

    public List<string> AllowedScopes { get; set; } = new();

    public List<string> AllowedGrantTypes { get; set; } = new();

    public List<string> RedirectUris { get; set; } = new();

    public List<string> PostLogoutRedirectUris { get; set; } = new();

    public List<ClientSecretMaskedDto> ClientSecrets { get; set; } = new();
}

public record ClientCreateUpdateDto
{
    [Required]
    public string? ClientName { get; set; }

    public string? Description { get; set; }

    public string? LogoUri { get; set; }

    public string? ClientUri { get; set; }

    public bool AllowAccessTokensViaBrowser { get; set; }

    public List<string> AllowedScopes { get; set; } = new();

    public List<string> AllowedGrantTypes { get; set; } = new();

    public List<string> RedirectUris { get; set; } = new();

    public List<string> PostLogoutRedirectUris { get; set; } = new();
}