namespace FitnessCentar.Email.Models
{
    public class EmailLog
    {
        public string Id { get; set; } // Neo4j koristi ID string tipa
        public string Recipient { get; set; } // Email korisnika
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
