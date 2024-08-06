using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Predictrix.Services
{
    public interface IUserService
    {
        string? GetUserId();

        string? GetUserEmail();
    }

    public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
    {
        public string? GetUserId()
        {
            return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string? GetUserEmail()
        {
            return httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Email);
        }
    }
}