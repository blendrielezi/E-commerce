using ProjektSezon2.Models;
using System.Security.Claims;

namespace ProjektSezon2.Services
{
    public interface ISessionService
    {
        Task CreateSessionAsync(ClaimsPrincipal user);//krijon sesion per perdoruesin aktual
        Task EndSessionAsync(ClaimsPrincipal user);//mbyll sesionin aktiv të përdoruesit.
        Task<IEnumerable<Session>> GetUserSessionsAsync(string userId); //kthen listën e sesioneve për përdoruesin me ID-në e dhënë.
    }
}