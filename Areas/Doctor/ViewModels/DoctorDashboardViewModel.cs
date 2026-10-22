using System;
using System.Collections.Generic;

namespace NMB_HLabSys.Areas.Doctor.ViewModels;

public sealed class DoctorDashboardViewModel
{
    public int PatientCount { get; set; }
    public int TestRequestCount { get; set; }
    public int CompletedResultsCount { get; set; }
    public int AbnormalResultsCount { get; set; }
    public int SubmittedCount { get; set; }
    public int InProgressCount { get; set; }
    public int ReadyToReleaseCount { get; set; }
    public int ActiveRequestCount { get; set; }
    public List<DashboardRequestRowViewModel> RecentRequests { get; set; } = new();
    public List<DashboardChartItemViewModel> StatusChart { get; set; } = new();
    public List<DashboardChartItemViewModel> MonthlyChart { get; set; } = new();
}

public sealed class DashboardRequestRowViewModel
{
    public int TestRequestId { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string PatientName { get; set; } = "Patient record unavailable";
    public DateTime RequestDate { get; set; }
    public string Urgency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class DashboardChartItemViewModel
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}