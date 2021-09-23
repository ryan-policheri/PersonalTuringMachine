using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace PersonalTuringMachine.Converters
{
    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class TapeNumberToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number = int.Parse(value.ToString());
            if (number <= 0) return new SolidColorBrush(Colors.White);
            else
            {
                if (number % 5 == 0) return new SolidColorBrush(Colors.LightPink);
                if (number % 4 == 0) return new SolidColorBrush(Colors.LightGray);
                if (number % 3 == 0) return new SolidColorBrush(Colors.BlanchedAlmond);
                if (number % 2 == 0) return new SolidColorBrush(Colors.Azure);
                else return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
