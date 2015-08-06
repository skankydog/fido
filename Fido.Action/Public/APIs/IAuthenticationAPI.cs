using System;

namespace Fido.Action
{
    public interface IAuthenticationAPI
    {
        void SignIn(Guid UserId, string Fullname, bool RememberMe);
        void SignOut();

        bool Authenticated { get; }
        Guid AuthenticatedId { get; }
        string LoggedInCredentialState { get; set; }
    }
}
