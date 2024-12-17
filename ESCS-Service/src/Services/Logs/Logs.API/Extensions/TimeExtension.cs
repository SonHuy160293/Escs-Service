namespace Logs.API.Extensions
{
    public static class TimeExtension
    {
        public static List<string> GetDaysInMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month); // Get the number of days in the month
            var dates = new List<string>();

            for (int day = 1; day <= days; day++)
            {
                dates.Add(new DateTime(year, month, day).ToString("dd/MM"));
            }

            return dates;
        }

        public static List<int> GetMonthsInYear(int year)
        {
            var months = new List<int>();
            if (year == DateTime.UtcNow.Year)
            {
                var currentMonth = DateTime.UtcNow.Month;
                for (int i = 1; i <= currentMonth; i++)
                {
                    months.Add(i);
                }
            }
            else
            {
                for (int i = 1; i <= 12; i++)
                {
                    months.Add(i);
                }
            }
            return months;
        }

        public static List<int> GetHoursInday(DateTime day)
        {
            var hours = new List<int>();



            for (int i = 0; i <= day.Hour; i++)
            {
                hours.Add(i);
            }
            return hours;
        }


    }
}
