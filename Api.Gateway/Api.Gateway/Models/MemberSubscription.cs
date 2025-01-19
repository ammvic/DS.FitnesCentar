namespace Api.Gateway.Models
{
    public class MemberSubscription
    {
        public string Id { get; set; } // Identifikator pretplate
        public string MemberId { get; set; } // ID člana na kojeg se odnosi pretplata
        public string Type { get; set; } // Tip pretplate, npr. "Monthly", "Yearly"
        public decimal Price { get; set; } // Cena pretplate
        public DateTime StartDate { get; set; } // Datum početka pretplate
        public DateTime EndDate { get; set; } // Datum završetka pretplate
        public bool IsActive { get; set; } // Status pretplate
    }
}
