using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Sync.Domain;

namespace Sync.Configuration;

public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly AuthenticationService _authenticationService;
    private IConfiguration _config;

    public CustomAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        AuthenticationService authenticationService,
        IConfiguration config
    ) : base(options, logger, encoder, clock)
    {
        _authenticationService = authenticationService;
        _config = config;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var pathPrefixes = _config.GetSection("UnauthenticatedPathPrefixes").Get<List<string>>() ?? [];
        foreach (var pathPrefix in pathPrefixes)
        {
            if (Request.Path.Value != null && Request.Path.Value.StartsWith(pathPrefix))
            {
                return AuthenticateResult.NoResult();
            }
        }

        var authUserHeader = Request.Headers["x-auth-user"].ToString();
        if (!string.IsNullOrWhiteSpace(authUserHeader))
        {
            if (!authUserHeader.Contains('@'))
            {
                Response.StatusCode = 401;
                return AuthenticateResult.Fail("Please append \"@https://host\" to your username to define the server url.");
            }

            var parts = authUserHeader.Split('@', StringSplitOptions.TrimEntries);
            var username = string.Join('@', parts[0..^1]);
            var hostUrl = parts.Last();

            if (!username.Contains(':'))
            {
                Response.StatusCode = 401;
                return AuthenticateResult.Fail("Please append \":<password>@https://host\" to your username to define your password.");
            }

            parts = username.Split(':', StringSplitOptions.TrimEntries);
            username = parts[0];
            var password = string.Join(':', parts[1..]);

            var user = new User(hostUrl, username, password);

            if (await _authenticationService.AuthenticateAsync(user))
            {
                var claims = new[] { new Claim(ClaimTypes.Name, user.Username, null, user.HostUrl) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var claimsPrincipal = new ClaimsPrincipal(identity);
                return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
            }

            Response.StatusCode = 401;
            return AuthenticateResult.Fail("Invalid username and/or password");
        }
        else
        {
            Response.StatusCode = 401;
            return AuthenticateResult.Fail("Not authenticated");
        }
    }
}
