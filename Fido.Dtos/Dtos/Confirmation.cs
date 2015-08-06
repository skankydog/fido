using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class Confirmation : Dto
    {
        public string Type { get; set; }
        public Guid UserId { get; set; }
        public string EmailAddress { get; set; }
        public DateTime QueuedUTC { get; set; }
        public DateTime? SentUTC { get; set; }
        public DateTime? ReceivedUTC { get; set; }
    }
}