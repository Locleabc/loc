using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using TopCom;
using TopUI.Define;

namespace TopUI.Converters
{
    public class CellStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ECellStatus && value != null)
            {
                ECellStatus status = (ECellStatus)value;

                SolidColorBrush color = CColors.CellColor.ElementAt((int)status).Value;

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
