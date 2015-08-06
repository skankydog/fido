using System;

namespace Fido.WebUI.Flash.Messages
{
    [Serializable]
    public class FlashMessage
    {
        internal FlashMessage()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public string Id { get; set; }
        //public string Title { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }

        public object Data { get; set; }
    }
}
