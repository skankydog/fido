﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities
{
    public class Confirmation : Entity
    {
        public Guid UserId { get; set; }
        public string ConfirmType { get; set; }
        public string EmailAddress { get; set; }
        public DateTime QueuedUTC { get; set; }
        public DateTime? SentUTC { get; set; }
        public DateTime? ReceivedUTC { get; set; }

        private const string CONFIRMED = "Confirmed";
        private const string SENT = "Sent";
        private const string QUEUED = "Queued";
        private const string ERROR = "Error";

        public string State
        {
            get
            {
                if (ReceivedUTC != null)
                    return CONFIRMED;

                if (SentUTC != null)
                    return SENT;

                if (QueuedUTC != null)
                    return QUEUED;

                return ERROR; // all are null
            }
        }

        public bool Deletable
        {
            get
            {
                if (ReceivedUTC == null)
                    return true;

                return false;
            }
        }
    }
}
