using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Service;

namespace Fido.ViewModel.Tests.Mocks
{
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
