namespace TransportRequestSystem.Models
{
    public class ReportFilter
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<ApplicationStatus>? SelectedStatuses { get; set; }
        public string? OrganizationUnit { get; set; }
        public bool IncludeHistory { get; set; }
    }

    public class ReportData
    {
        public int TotalApplications { get; set; }
        public double AvgCompletionTime { get; set; }
        public Dictionary<string, int> StatusDistribution { get; set; } = new();
        public Dictionary<string, int> VehicleUsage { get; set; } = new();
        public Dictionary<string, int> DailyStats { get; set; } = new();
        public List<Application> Applications { get; set; } = new();
    }
}