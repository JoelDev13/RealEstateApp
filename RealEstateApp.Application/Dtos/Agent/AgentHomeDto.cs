namespace RealEstateApp.Application.Dtos.Agent
{
    public class AgentHomeDto
    {
        public string AgentId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public byte[]? PhotoData { get; set; }  // Handler para foto
        public string PhotoContentType { get; set; } = string.Empty;  // Content-Type
    }
}
