using Newtonsoft.Json;
using System;
using TimeRecorder.Core.Constants;

namespace TimeRecorder.Core.Models
{
    public class TimeCard
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("recordDate")]
        public DateTime RecordDate { get; set; }
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("recordType")]
        public RecordType RecordType { get; set; }

        public TimeCard()
        {
        }
        public TimeCard(DateTime now, RecordType recordType, string userId)
        {
            RecordDate = now;
            RecordType = recordType;
            UserId = userId;
            Id = $"{RecordDate:yyyy-MM-dd}-{recordType}";
        }

        public string CreateReportTime() => RecordType switch
        {
            RecordType.ClockIn => CreateClockInReportTime(),
            RecordType.ClockOut => CreateClockOutReportTime(),
            _ => throw new ArgumentException(),
        };

        private string CreateClockOutReportTime()
            => RoundDown(RecordDate, TimeSpan.FromMinutes(15)).ToString("HH:mm");
        private string CreateClockInReportTime()
            => RoundUp(RecordDate, TimeSpan.FromMinutes(15)).ToString("HH:mm");

        private static DateTime RoundUp(DateTime input, TimeSpan interval)
            => new DateTime(((input.Ticks + interval.Ticks - 1) / interval.Ticks) * interval.Ticks, input.Kind);

        private static DateTime RoundDown(DateTime input, TimeSpan interval)
        => new DateTime((((input.Ticks + interval.Ticks) / interval.Ticks) - 1) * interval.Ticks, input.Kind);
    }
}
