using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Predictrix.Application.Commands;
using Serilog;

namespace Predictrix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(ISender mediator, IConfiguration configuration) : ControllerBase
    {
        [HttpGet("signin")]
        public IActionResult GoogleSignInLink()
        {
            var clientId = configuration["Authentication:Google:ClientId"];
            var redirectUri = configuration["Authentication:Google:RedirectUri"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
                throw new Exception("Google client ID or redirect URI not configured");

            var url =
                $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&response_type=code&scope=openid%20profile%20email&redirect_uri={Uri.EscapeDataString(redirectUri)}";
            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> GoogleCallback(string code)
        {
            var clientId = configuration["Authentication:Google:ClientId"];
            var clientSecret = configuration["Authentication:Google:ClientSecret"];
            var redirectUri = configuration["Authentication:Google:RedirectUri"];

            var tokenResponse = await new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }
            }).ExchangeCodeForTokenAsync("", code, redirectUri, CancellationToken.None);

            // Get the ID token from the response
            var idToken = tokenResponse.IdToken;

            // Verify the ID token
            var validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            Log.Information("Validated ID token for: {Name}", validPayload.Name);

            var userId = validPayload.Subject;
            var email = validPayload.Email;
            var name = validPayload.Name;
            var photoUrl = validPayload.Picture;

            Log.Information("Creating or updating user: {UserId} {Email} {Name} {PhotoUrl}", userId, email, name,
                photoUrl);

            var result = await mediator.Send(new CreateOrUpdateUserCommand
            {
                UserId = userId,
                Email = email,
                DisplayName = name,
                PhotoUrl = photoUrl
            });

            if (result.IsFail)
                return BadRequest(result.Message);

            var (token, expires) = GenerateAppToken(configuration, userId, email);

            Log.Information("Received token: {Token}", token);

            return Redirect(
                $"{configuration["Authentication:Frontend:RedirectUri"]}?token={token}&expires={expires:o}");
        }

        private static Tuple<string, DateTime> GenerateAppToken(IConfiguration configuration, string userId,
            string email)
        {
            var secretKey = configuration["Authentication:Jwt:SecretKey"];
            var issuer = configuration["Authentication:Jwt:Issuer"];
            var audience = configuration["Authentication:Jwt:Audience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                throw new Exception("JWT configuration not found");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var claim in claims)
                Log.Information("Claim: {Type} = {Value}", claim.Type, claim.Value);

            const int expiresInHours = 672; // 4 weeks
            var expires = DateTime.UtcNow.AddHours(expiresInHours);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            Log.Information("Token expiration: {Expires}", token.ValidTo);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new Tuple<string, DateTime>(tokenString, expires);
        }
    }
}