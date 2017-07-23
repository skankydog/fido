using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Fido.Core;
using Fido.Service;

namespace Fido.ViewModel.Tests.Mocks
{
    [ExcludeFromCodeCoverage]
    public class MockAuthenticationAPI : IAuthenticationAPI
    {
        private Guid UserId = Guid.Empty;

        public void SignIn(Guid UserId, string Fullname, bool RememberMe)
        {
            this.UserId = UserId;
        }

        public void SignOut()
        {
            this.UserId = Guid.Empty;
        }

        public bool Authenticated
        {
            get { return UserId != Guid.Empty; }
        }

        public Guid AuthenticatedId
        {
            get { return UserId; }
        }

        public string LoggedInCredentialState
        {
            get
            {
                return "Active";
            }
            set
            {
                ;
            }
        }
    }
}
