using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TopCom.Models;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for SingleRecipeInfo.xaml
    /// </summary>
    public partial class SingleRecipeInfo : UserControl
    {
        public SingleRecipeInfo()
        {
            InitializeComponent();
        }

        public RecipeInfo Recipe
        {
            get { return (RecipeInfo)GetValue(RecipeProperty); }
            set { SetValue(RecipeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipeProperty =
            DependencyProperty.Register(nameof(Recipe), typeof(RecipeInfo), typeof(SingleRecipeInfo), new PropertyMetadata(null));

        public bool IsHeader
        {
            get { return (bool)GetValue(IsHeaderProperty); }
            set { SetValue(IsHeaderProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHeaderProperty =
            DependencyProperty.Register(nameof(IsHeader), typeof(bool), typeof(SingleRecipeInfo), new PropertyMetadata(false));
    }
}
