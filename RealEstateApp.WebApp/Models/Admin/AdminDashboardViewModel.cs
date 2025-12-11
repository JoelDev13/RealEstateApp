namespace RealEstateApp.WebApp.Models
{
    public class AdminDashboardViewModel
    {
        // ============ PROPIEDADES ============
        public int TotalProperties { get; set; }
        public int AvailableProperties { get; set; }
        public int SoldProperties { get; set; }

        // ============ ADMINISTRADORES ============
        public int TotalAdmins { get; set; }
        public int ActiveAdmins { get; set; }
        public int InactiveAdmins { get; set; }

        // ============ AGENTES ============
        public int TotalAgents { get; set; }
        public int ActiveAgents { get; set; }
        public int InactiveAgents { get; set; }

        // ============ CLIENTES ============
        public int TotalClients { get; set; }
        public int ActiveClients { get; set; }
        public int InactiveClients { get; set; }

        // ============ DESARROLLADORES ============
        public int TotalDevelopers { get; set; }
        public int ActiveDevelopers { get; set; }
        public int InactiveDevelopers { get; set; }

        // ============ TIPOS DE VENTA ============
        public int TotalSaleTypes { get; set; }
        public int ActiveSaleTypes { get; set; }
        public int InactiveSaleTypes { get; set; }

        // ============ TIPOS DE PROPIEDAD ============
        public int TotalPropertyTypes { get; set; }
        public int ActivePropertyTypes { get; set; }
        public int InactivePropertyTypes { get; set; }

        // ============ MEJORAS ============
        public int TotalImprovements { get; set; }
        public int ActiveImprovements { get; set; }
        public int InactiveImprovements { get; set; }

        // ============ PROPIEDADES CALCULADAS ============
        public decimal AvailablePropertiesPercentage =>
            TotalProperties > 0 ? (decimal)AvailableProperties / TotalProperties * 100 : 0;

        public decimal SoldPropertiesPercentage =>
            TotalProperties > 0 ? (decimal)SoldProperties / TotalProperties * 100 : 0;

        public decimal ActiveAdminsPercentage =>
            TotalAdmins > 0 ? (decimal)ActiveAdmins / TotalAdmins * 100 : 0;

        public decimal ActiveAgentsPercentage =>
            TotalAgents > 0 ? (decimal)ActiveAgents / TotalAgents * 100 : 0;

        public decimal ActiveClientsPercentage =>
            TotalClients > 0 ? (decimal)ActiveClients / TotalClients * 100 : 0;

        public decimal ActiveDevelopersPercentage =>
            TotalDevelopers > 0 ? (decimal)ActiveDevelopers / TotalDevelopers * 100 : 0;

        // ============ TOTAL DE USUARIOS ============
        public int TotalUsers => TotalAdmins + TotalAgents + TotalClients + TotalDevelopers;
    }
}