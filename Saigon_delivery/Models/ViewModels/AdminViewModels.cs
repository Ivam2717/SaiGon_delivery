namespace Saigon_delivery.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
    }

    public class ShipperStatViewModel
    {
        public string ShipperName { get; set; } = null!;
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AvgRating { get; set; }
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = null!;
        public decimal Revenue { get; set; }
    }
}