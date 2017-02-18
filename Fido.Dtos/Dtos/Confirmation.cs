using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class Confirmation : Dto
    {
        public Guid UserId { get; set; }
        public string ConfirmType { get; set; }
        public string EmailAddress { get; set; }
        public DateTime QueuedUTC { get; set; }
        public DateTime? SentUTC { get; set; }
        public DateTime? ReceivedUTC { get; set; }
        public string State { get; set; }
        public bool Deletable { get; set; }
    }
}
