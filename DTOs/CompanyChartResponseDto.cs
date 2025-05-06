using System;

namespace AdeebBackend.DTOs;

public class CompanyChartResponseDto
{
    // e.g. ["Q1 2025","Q2 2025",…] or ["All Time"]
    public List<string> Labels { get; set; }

    // For combined/bar/turnover/retention charts:
    public CompanySeries? Data { get; set; }

    // Only for donut charts:
    public CompanySeries? CurrentStatus { get; set; }

    public DateTime LastUpdated { get; set; }
}

public class CompanySeries
{
    // If you’re not returning one of these, just leave the list empty or null
    public List<double>? Turnover { get; set; }
    public List<double>? Retention { get; set; }
}
