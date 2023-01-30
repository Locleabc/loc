using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using TopUI.Define;

namespace TopUI.Converters
{
    public class TextToTextForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && value != null)
            {
                string str = (string)value;

                SolidColorBrush color = new SolidColorBrush();

                if (str.ToLower().Contains("error") || str.ToLower().Contains("fatal"))
                {
                    color = new SolidColorBrush(Colors.Red);
                }
                else if (str.ToLower().Contains("warn"))
                {
                    color = new SolidColorBrush(Colors.Orange);
                }
                else if (str.ToLower().Contains("info"))
                {
                    color = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    color = new SolidColorBrush(Colors.Black);
                }

                return color;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
