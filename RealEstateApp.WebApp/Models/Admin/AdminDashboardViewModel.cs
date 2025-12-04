namespace RealEstateApp.WebApp.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalSaleTypes { get; set; }
        public int ActiveSaleTypes { get; set; }
        public int InactiveSaleTypes { get; set; }

        public int TotalPropertyTypes { get; set; }
        public int ActivePropertyTypes { get; set; }
        public int InactivePropertyTypes { get; set; }

        public int TotalImprovements { get; set; }
        public int ActiveImprovements { get; set; }
        public int InactiveImprovements { get; set; }

        public int TotalProperties { get; set; }
        public int TotalAgents { get; set; }
    }
}
