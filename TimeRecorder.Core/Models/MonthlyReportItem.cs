using TimeRecorder.Core.Constants;

namespace TimeRecorder.Core.Models
{
    public class MonthlyReportItem
    {
        public string Start { get; set; }
        public string End { get; set; }

        public MonthlyReportItem()
        {
        }

        public MonthlyReportItem(TimeCard timeCard)
        {
            var reportTime = timeCard.CreateReportTime();

            if (timeCard.RecordType == RecordType.ClockIn)
                Start = reportTime;
            else
                End = reportTime;
        }
    }
}
