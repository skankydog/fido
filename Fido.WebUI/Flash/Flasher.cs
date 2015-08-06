using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fido.WebUI.Flash.Messages;

namespace Fido.WebUI.Flash
{
    public class Flasher : IFlasher
    {
        private const string MVCFLASHMESSAGES = "___mvcflash_messages";
        private HttpContextBase m_HttpContext;
        private static readonly object m_Mutex = new object();

        private static class Severities
        {
            public static string Success = "success";
            public static string Info = "info";
            public static string Warning = "warning";
            public static string Error = "danger";
        }

        private IDictionary<string, FlashMessage> Messages
        {
            get
            {
                if (m_HttpContext == null || m_HttpContext.Handler == null)
                    m_HttpContext = new HttpContextWrapper(HttpContext.Current);

                if (m_HttpContext.Session[MVCFLASHMESSAGES] as IDictionary<string, FlashMessage> == null)
                {
                    lock (m_Mutex)
                    {
                        if (m_HttpContext.Session[MVCFLASHMESSAGES] as IDictionary<string, FlashMessage> == null)
                        {
                            m_HttpContext.Session[MVCFLASHMESSAGES] = new Dictionary<string, FlashMessage>();
                        }
                    }
                }

                return m_HttpContext.Session[MVCFLASHMESSAGES] as IDictionary<string, FlashMessage>;
            }
        }

        public void Success(string Description, object Data = null)
        {
            Push(Description, Severities.Success, Data);
        }

        public void Info(string Description, object Data = null)
        {
            Push(Description, Severities.Info, Data);
        }

        public void Warning(string Description, object Data = null)
        {
            Push(Description, Severities.Warning, Data);
        }

        public void Error(string Description, object Data = null)
        {
            Push(Description, Severities.Error, Data);
        }

        private void Push(string Description, string Severity, object Data)
        {
            var Message = new FlashMessage
            {
                Description = Description,
                Severity = Severity
            };

            if (Data != null)
            {
                Message.Data = Data;
            }

            Messages[Message.Id] = Message;
        }

        public virtual FlashMessage Pop()
        {
            var Message = Messages.Values.LastOrDefault();

            if (Message != null)
                Messages.Remove(Message.Id);

            return Message;
        }

        public virtual int Count { get { return Messages.Count; } }

        public void Clear()
        {
            while (Messages.Count > 0)
            {
                Pop();
            }
        }
    }
}
