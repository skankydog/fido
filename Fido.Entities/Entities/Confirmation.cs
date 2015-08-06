using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities
{
    public class Confirmation : Entity
    {
        public string ConfirmType { get; set; }
        public Guid UserId { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public DateTime QueuedUTC { get; set; }
        public DateTime? SentUTC { get; set; }
        public DateTime? ReceivedUTC { get; set; }
    }
}
