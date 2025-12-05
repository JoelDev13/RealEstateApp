namespace RealEstateApp.WebApp.Models
{
    public class AdminDashboardViewModel
    {
        // ============ PROPIEDADES ============
        public int TotalProperties { get; set; }
        public int AvailableProperties { get; set; }
        public int SoldProperties { get; set; }

        // ============ USUARIOS ============
        // Agentes
        public int TotalAgents { get; set; }
        public int ActiveAgents { get; set; }
        public int InactiveAgents { get; set; }

        // Clientes
        public int TotalClients { get; set; }
        public int ActiveClients { get; set; }
        public int InactiveClients { get; set; }

        // Desarrolladores
        public int TotalDevelopers { get; set; }
        public int ActiveDevelopers { get; set; }
        public int InactiveDevelopers { get; set; }

        // ============ CATÁLOGOS ============
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