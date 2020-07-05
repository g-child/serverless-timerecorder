using System;
using System.Collections.Generic;
using TimeRecorder.Core.Constants;

namespace TimeRecorder.Core.Models
{
    public class MonthlyRecord
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime ReportMonth { get; set; }
        public IDictionary<int, MonthlyReportItem> Report { get; set; }

        public MonthlyRecord()
        {
        }

        public MonthlyRecord(DateTime reportMonth, string userId)
        {
            Id = reportMonth.ToString("yyyy-MM");
            UserId = userId;
            ReportMonth = reportMonth;
            Report = new Dictionary<int, MonthlyReportItem>();
        }

        public void UpdateRecord(TimeCard dailyReport)
        {
            Report.TryGetValue(dailyReport.RecordDate.Day, out var target);
            var reportTime = dailyReport.CreateReportTime();

            if (dailyReport.RecordType == RecordType.ClockIn)
                target.Start = reportTime;
            else
                target.End = reportTime;
        }
    }
}
