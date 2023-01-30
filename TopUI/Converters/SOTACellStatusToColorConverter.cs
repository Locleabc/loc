using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using TopCom;

namespace TopUI.Converters
{
    [ValueConversion(typeof(object), typeof(Dictionary<string, SolidColorBrush>))]
    public class TrayCellStatusListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<string, SolidColorBrush> ColorList = new Dictionary<string, SolidColorBrush>();

            if (value.GetType().GetGenericArguments().Count() > 0)
            {
                var type = value.GetType().GetGenericArguments()[0];
                if (type.IsEnum)
                {
                    var values = Enum.GetValues(type);

                    foreach ( var v in values )
                    {
                        ColorList.Add(v.ToString(), CCellColors.Background[v.ToString()]);
                    }

                    return ColorList;
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class SOTACellStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                SolidColorBrush color = CCellColors.Background[value.ToString()];

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
