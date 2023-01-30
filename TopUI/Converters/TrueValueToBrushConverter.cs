using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TopUI.Define;

namespace TopUI.Converters
{
    public class BooleanToButtonHightLightBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                {
                    if (parameter != null)
                    {
                        return parameter;
                    }
                    else
                    {
                        return Brushes.Green;
                    }
                }
                else
                {
                    return Brushes.Black;
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
