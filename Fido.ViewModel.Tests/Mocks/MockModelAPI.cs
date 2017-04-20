using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Service;

namespace Fido.ViewModel.Tests.Mocks
{
    [ExcludeFromCodeCoverage]
    public class MockModelAPI : IModelAPI
    {
        private bool RaisedPropertyError = false;
        private bool RaisedModelError = false;

        public bool HasPropertyError { get { return RaisedPropertyError;  } }
        public bool HasModelError { get { return RaisedModelError; } }
        public bool HasAnyError { get { return RaisedPropertyError || RaisedModelError; } }

        public void PropertyError(string Property, string Message)
        {
            RaisedPropertyError = true;
        }

        public void ModelError(string Message)
        {
            RaisedModelError = true;
        }

        public bool ModelStateIsValid()
        {
            return true;
        }
    }
}
