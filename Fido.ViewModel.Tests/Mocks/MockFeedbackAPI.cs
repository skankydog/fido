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
    public enum FeedbackState
    {
        Unknown = 0,
        Success,
        Failure
    }

    public class MockFeedbackAPI : IFeedbackAPI
    {
        public FeedbackState State { get; private set; }

        public MockFeedbackAPI()
        {
            State = FeedbackState.Unknown;
        }

        public void DisplayInfo(string Message)
        {
            ; // No change to state
        }

        public void DisplaySuccess(string Message)
        {
            State = FeedbackState.Success;
        }

        public void DisplayWarning(string Message)
        {
            State = FeedbackState.Failure;
        }

        public void DisplayError(string Message)
        {
            State = FeedbackState.Failure;
        }

        public Mode Mode
        {
            get { return Mode.Production; }
        }
    }
}
