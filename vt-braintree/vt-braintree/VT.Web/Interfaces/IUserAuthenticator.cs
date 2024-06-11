using VT.Web.Components;

namespace VT.Web.Interfaces
{
    public interface IUserAuthenticator
    {
        LoginResult AuthenticateUser(string username, string password); 
    }
}
