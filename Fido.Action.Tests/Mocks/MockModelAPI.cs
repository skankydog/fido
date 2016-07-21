using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Service;

namespace Fido.Action.Tests.Mocks
{
    public class MockModelAPI : IModelAPI
    {
        public void PropertyError(string Property, string Message)
        {
            ;
        }

        public void ModelError(string Message)
        {
            ;
        }

        public bool ModelStateIsValid()
        {
            return true;
        }
    }
}
