using IdentityModel;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atomic.IdentityServer.Api.Extensions;

public static class ClientExtensions
{
    public static Client MapFrom(this Client client, ClientCreateUpdateDto input)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));

        client.ClientName = input.ClientName;
        client.Description = input.Description ?? input.ClientName;
        client.LogoUri = input.LogoUri;
        client.ClientUri = input.ClientUri;

        client.RedirectUris.Clear();
        foreach (var redirectUri in input.RedirectUris)
        {
            client.RedirectUris.Add(new ClientRedirectUri
            {
                Client = client,
                RedirectUri = redirectUri,
            });
        }

        client.AllowedScopes.Clear();
        foreach (var scope in input.AllowedScopes)
        {
            client.AllowedScopes.Add(new ClientScope
            {
                Client = client,
                Scope = scope,
            });
        }

        client.AllowedGrantTypes.Clear();
        foreach (var grantType in input.AllowedGrantTypes)
        {
            client.AllowedGrantTypes.Add(new ClientGrantType
            {
                Client = client,
                GrantType = grantType,
            });
        }

        client.PostLogoutRedirectUris.Clear();
        foreach (var redirectUri in input.PostLogoutRedirectUris)
        {
            client.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri
            {
                Client = client,
                PostLogoutRedirectUri = redirectUri,
            });
        }

        return client;
    }

    public static IQueryable<Client> WithDetails(this DbSet<Client> clients)
    {
        return clients.Include(c => c.ClientSecrets)
            .Include(c => c.RedirectUris)
            .Include(c => c.PostLogoutRedirectUris)
            .Include(c => c.AllowedScopes)
            .Include(c => c.AllowedGrantTypes);
    }

    public static ClientSecretDto GenerateNewSecret(this Client client)
    {
        var rawSecretValue = Guid.NewGuid().ToString("D");
        var secret = new ClientSecret
        {
            Client = client,
            Value = rawSecretValue.ToSha256(),
            Description = $"for client: {client.ClientName}",
        };

        client.ClientSecrets ??= new List<ClientSecret>();
        client.ClientSecrets.Add(secret);

        return new ClientSecretDto
        {
            Description = secret.Description,
            Created = secret.Created,
            Value = rawSecretValue,
        };
    }
}