namespace RealEstateApp.Application.Dtos.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalProperties { get; set; }
        public int AvailableProperties { get; set; }
        public int SoldProperties { get; set; }
        public int TotalAgents { get; set; }
        public int ActiveAgents { get; set; }
        public int InactiveAgents { get; set; }
        public int TotalClients { get; set; }
        public int ActiveClients { get; set; }
        public int InactiveClients { get; set; }
        public int TotalDevelopers { get; set; }
        public int ActiveDevelopers { get; set; }
        public int InactiveDevelopers { get; set; }
        public int TotalSaleTypes { get; set; }
        public int ActiveSaleTypes { get; set; }
        public int InactiveSaleTypes { get; set; }
        public int TotalPropertyTypes { get; set; }
        public int ActivePropertyTypes { get; set; }
        public int InactivePropertyTypes { get; set; }
        public int TotalImprovements { get; set; }
        public int ActiveImprovements { get; set; }
        public int InactiveImprovements { get; set; }
    }
}