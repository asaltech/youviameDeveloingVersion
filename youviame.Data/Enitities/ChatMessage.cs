using System;

namespace youviame.Data.Enitities {
    public class ChatMessage : BaseEntity {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public double DateTime { get; set; }
        public Guid MatchId { get; set; }
    }
}