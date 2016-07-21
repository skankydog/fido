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
    public class MockFeedbackAPI : IFeedbackAPI
    {
        public void DisplayInfo(string Message)
        {
            ;
        }

        public void DisplaySuccess(string Message)
        {
            ;
        }

        public void DisplayWarning(string Message)
        {
            ;
        }

        public void DisplayError(string Message)
        {
            ;
        }

        public Mode Mode
        {
            get { return Mode.Production; }
        }
    }
}
