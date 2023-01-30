using System.Collections.Generic;
using System.Windows.Media;
using TopCom;

namespace TopUI.Converters
{
    public static class CCellColors
    {
        public static Dictionary<string, SolidColorBrush> Background => new Dictionary<string, SolidColorBrush>
        {
            {
                ECellSimpleStatus.Ready.ToString(), new SolidColorBrush(Colors.Snow)
            },
            {
                ECellSimpleStatus.Skip.ToString(), new SolidColorBrush(Colors.LightGray)
            },
            {
                ECellSimpleStatus.Fail.ToString(), new SolidColorBrush(Colors.Tomato)
            },
            {
                ECellSimpleStatus.Pass.ToString(), new SolidColorBrush(Colors.Lime)
            },
            {
                ECellAdvancedStatus.Empty.ToString(), new SolidColorBrush(Colors.Snow)
            },
            {
                ECellAdvancedStatus.Processing.ToString(), new SolidColorBrush(Colors.LightYellow)
            },
            {
                ECellAdvancedStatus.NGVision.ToString(), new SolidColorBrush(Colors.OrangeRed)
            },
            {
                ECellAdvancedStatus.OKVision.ToString(), new SolidColorBrush(Colors.Blue)
            },
            {
                ECellAdvancedStatus.NGPickOrPlace.ToString(), new SolidColorBrush(Colors.Red)
            },
            {
                ECellAdvancedStatus.OK.ToString(), new SolidColorBrush(Colors.Green)
            }
        };
    }
}
