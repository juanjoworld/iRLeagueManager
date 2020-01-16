﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using iRLeagueManager.Enums;

namespace iRLeagueManager.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            DateTime date = (DateTime)value;
            return date.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string dateString = (string)value;
            DateTime.TryParse(dateString, out DateTime result);
            return result;
        }
    }

    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.TimeOfDay.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string timeString = (string)value;
            TimeSpan.TryParse(timeString, out TimeSpan result);
            return DateTime.MinValue.Add(result);
        }
    }

    public class WeekDayFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!int.TryParse((string)parameter, out int thisWeekDay))
            {
                return null;
            }

            if (value is WeekDaysFlag weekDayFlag)
            {
                return weekDayFlag.HasFlag((WeekDaysFlag)(1 << thisWeekDay));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!int.TryParse((string)parameter, out int thisWeekDay))
            {
                return 0;
            }

            if (value is bool selected)
            {
                if (selected)
                {
                    return (WeekDaysFlag)(1 << thisWeekDay);
                }
                else
                {
                    return (WeekDaysFlag)(~(1 << thisWeekDay));
                }
            }
            return 0;
        }
    }

    public class BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return ((bool)value) ? 1 : 0;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return (((double)value) >= 0.5) ? true : false;
            }
            return null;
        }
    }

    public class TimeComponentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value).ToString("00");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (int.TryParse((string)value, out int result))
                {
                    return result;
                }
            }
            return null;
        }
    }

    public class SliderValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int numberValue = (int)value;
            return (bool)(numberValue > 50);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = (bool)value;
            return status ? 10 : 1; 
        }
    }
}
