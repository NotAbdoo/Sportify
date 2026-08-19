using System;

namespace Sportify.Models
{
    public class UserMessage
    {
        public int UserMessageID { get; set; }
        public int UserID { get; set; }
        public string MessageText { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}
