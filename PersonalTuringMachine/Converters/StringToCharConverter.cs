using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PersonalTuringMachine.Converters
{
    [ValueConversion(typeof(String), typeof(char))]
    public class StringToCharConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value.ToString();
            return str?.First();
        }
    }
}
