namespace CRM.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToRelativeTime(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan <= TimeSpan.FromSeconds(60))
                return "just now";
            if (timeSpan <= TimeSpan.FromMinutes(60))
                return $"{timeSpan.Minutes} minutes ago";
            if (timeSpan <= TimeSpan.FromHours(24))
                return $"{timeSpan.Hours} hours ago";
            if (timeSpan <= TimeSpan.FromDays(7))
                return $"{timeSpan.Days} days ago";
            if (timeSpan <= TimeSpan.FromDays(30))
                return $"{timeSpan.Days / 7} weeks ago";
            if (timeSpan <= TimeSpan.FromDays(365))
                return $"{timeSpan.Days / 30} months ago";

            return $"{timeSpan.Days / 365} years ago";
        }

        public static DateTime StartOfDay(this DateTime date)
            => date.Date;

        public static DateTime EndOfDay(this DateTime date)
            => date.Date.AddDays(1).AddTicks(-1);

        public static bool IsWeekend(this DateTime date)
            => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }
}
