using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TwitterApp.Helpers
{
    public class AgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string) && value is DateTime)
            {
                DateTime n = DateTime.UtcNow;
                DateTime v = (DateTime)value;

                var d = n - v;

                if (d.TotalDays > 1)
                    return days((int)Math.Floor(d.TotalDays));

                if (d.TotalHours > 1)
                    return hours((int)Math.Floor(d.TotalHours));

                if (d.TotalMinutes > 1)
                    return minutes((int)Math.Floor(d.TotalMinutes));

                else
                    return seconds((int)Math.Floor(d.TotalSeconds));
            }
            return null;
        }

        private static string days(int value)
        {
            string h = "дней";
            var x = value % 100;
            if (10 > x || x > 20)
            {
                switch (value % 10)
                {
                    case 1: h = "день"; break;
                    case 2:
                    case 3:
                    case 4: h = "дня"; break;
                }
            }
            return $"{value.ToString("N0")} {h} назад";
        }

        private static string hours(int value)
        {
            string h = "часов";
            if (10 > value || value > 20)
            {
                switch (value % 10)
                {
                    case 1: h = "час"; break;
                    case 2:
                    case 3:
                    case 4: h = "часа"; break;
                }
            }
            return $"{value} {h} назад";
        }

        private static string minutes(int value)
        {
            string h = "минут";
            if (10 > value || value > 20)
            {
                switch (value % 10)
                {
                    case 1: h = "минута"; break;
                    case 2:
                    case 3:
                    case 4: h = "минуты"; break;
                }
            }
            return $"{value} {h} назад";
        }

        private static string seconds(int value)
        {
            string h = "секунд";
            if (10 > value || value > 20)
            {
                switch (value % 10)
                {
                    case 1: h = "секунда"; break;
                    case 2:
                    case 3:
                    case 4: h = "секунды"; break;
                }
            }
            return $"{value} {h} назад";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
