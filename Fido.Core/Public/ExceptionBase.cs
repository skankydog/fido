using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Fido.Core
{
    [Serializable]
    public abstract class ExceptionBase : Exception, ISerializable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ExceptionBase()
        { }

        public ExceptionBase(string Message)
            : base(Message)
        { }

        public ExceptionBase(string Message, Exception Inner)
            : base(Message, Inner)
        {
            using (new FunctionLogger(Log))
            {
                Log.ErrorFormat("Exception Message: {0}", Message);
                Log.ErrorFormat("Exception Inner: {0}", Inner);
            }
        }

        // Constructor needed for serialization when exception propagates from a remoting
        // server to the client
        protected ExceptionBase(SerializationInfo Info, StreamingContext Context)
            : base(Info, Context)
        {
            Log.Debug("Serializing BaseException...");
        }
    }
}
