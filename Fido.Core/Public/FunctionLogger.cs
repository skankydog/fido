using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Core
{
    public class FunctionLogger : IDisposable
    {
        private log4net.ILog Log { get; set; }
        private string Name { get; set; }

        // When targetting 4.5/C# 5:
        // public void LogWrapper(log4net.ILog Log, [CallerMemberName] string Caller = null)
        //     The compiler will add the caller name at compile-time, so will have zero cost
        //     as far as execution goes.
        public FunctionLogger(log4net.ILog Log)
        {
            this.Log = Log;

            if (!this.Log.IsDebugEnabled) // StackTrace, below, is expensive...
                return;

            Name = new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod().ToString();
            this.Log.DebugFormat("[ Top of {0} ]", this.Name);
        }

        public void Dispose()
        {
            if (!this.Log.IsDebugEnabled)
                return;

            this.Log.DebugFormat("[ Bottom of {0} ]", this.Name);
        }
    }
}
