using IdentityServer4.EntityFramework.Entities;

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

        client.RedirectUris = new List<ClientRedirectUri>(input.RedirectUris.Count);
        client.RedirectUris.Clear();
        foreach (var redirectUri in input.RedirectUris)
        {
            client.RedirectUris.Add(new ClientRedirectUri
            {
                Client = client,
                RedirectUri = redirectUri,
            });
        }

        client.AllowedScopes = new List<ClientScope>(input.AllowedScopes.Count);
        foreach (var scope in input.AllowedScopes)
        {
            client.AllowedScopes.Add(new ClientScope
            {
                Client = client,
                Scope = scope,
            });
        }

        client.AllowedGrantTypes = new List<ClientGrantType>(input.AllowedGrantTypes.Count);
        foreach (var grantType in input.AllowedGrantTypes)
        {
            client.AllowedGrantTypes.Add(new ClientGrantType
            {
                Client = client,
                GrantType = grantType,
            });
        }

        client.PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>(input.PostLogoutRedirectUris.Count);
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
}