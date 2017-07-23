using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fido.ViewModel.Tests.Mocks
{
    [ExcludeFromCodeCoverage]
    public class MockResult
    {
        public ResultType ResultType;
    }

    public enum ResultType
    {
        Success = 0,
        Authentication,
        PasswordReset,
        Error,
        Invalid
    }
}
