using System;

namespace Fido.Action
{
    public enum Mode
    {
        Development = 0,
        Test,
        Production
    }

    public interface IFeedbackAPI
    {
        void DisplayInfo(string Message);
        void DisplaySuccess(string Message);
        void DisplayWarning(string Message);
        void DisplayError(string Message);

        Mode Mode { get; } // Should not really be here - needs to be moved
    }
}
