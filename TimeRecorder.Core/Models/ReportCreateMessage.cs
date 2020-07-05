using Newtonsoft.Json;
using System;

namespace TimeRecorder.Core.Models
{
    public class ReportCreateMessage
    {
        public DateTime TargetMonth { get; set; }
        public string UserId { get; set; }

        [JsonIgnore]
        public string MonthlyRecordId
            => TargetMonth.ToString("yyyy-MM");
    }
}
