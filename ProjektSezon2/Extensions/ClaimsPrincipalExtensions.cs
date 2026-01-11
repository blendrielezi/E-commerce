// ProjektSezon2/Extensions/ClaimsPrincipalExtensions.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ProjektSezon2.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Retrieves the remote IP address for the current user via IHttpContextAccessor.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal of the signed-in user.</param>
        /// <param name="httpContextAccessor">Injected IHttpContextAccessor.</param>
        /// <returns>IP address as string, or empty if unavailable.</returns>
        public static string GetIpAddress(this ClaimsPrincipal principal, IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?
                       .Connection?
                       .RemoteIpAddress?
                       .ToString()
                   ?? string.Empty;
        }
    }
}
