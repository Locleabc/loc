using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using TopUI.Controls;
using VCM_FullAssy.Define;
using VCM_FullAssy.MVVM.ViewModels;

namespace VCM_FullAssy.MVVM.Views
{
    /// <summary>
    /// Interaction logic for RecipeView.xaml
    /// </summary>
    public partial class RecipeView : UserControl
    {
        public int Index { get; set; } = 0;

        public RecipeView()
        {
            InitializeComponent();
        }

        private void LoadAllRecipe()
        {
            LoadRecipe(CDef.CommonRecipe, CommonRecipe_StackPanel, CommonRecipeOption_UniformGrid);
            LoadRecipe(CDef.LeftTrayRecipe, LeftTrayRecipe_StackPanel, LeftTrayRecipeOption_UniformGrid);
            LoadRecipe(CDef.RightTrayRecipe, RightTrayRecipe_StackPanel, RightTrayRecipeOption_UniformGrid);
            LoadRecipe(CDef.TransferRecipe, TransferRecipe_StackPanel, TransferRecipeOption_UniformGrid);
            LoadRecipe(CDef.HeadRecipe, HeadRecipe_StackPanel, HeadRecipeOption_UniformGrid);
            LoadRecipe(CDef.UpperVisionRecipe, UpperVisionRecipe_StackPanel, UpperVisionRecipeOption_UniformGrid);
            LoadRecipe(CDef.UnderVisionRecipe, UnderVisionRecipe_StackPanel, UnderVisionRecipeOption_UniformGrid);
        }

        private void LoadRecipe<T>(T recipe, Panel mainPanel, Panel optionPanel = null)
        {
            PropertyInfo[] props = typeof(T).GetProperties();

            mainPanel.Children.Add(new SingleRecipe { IsHeader = true });

            foreach (PropertyInfo prop in props)
            {
                if (prop.Name.Contains("NULL_SPACE"))
                {
                    mainPanel.Children.Add(new SingleRecipe { IsNullSpace = true });

                    continue;
                }

                object[] attrs = prop.GetCustomAttributes(true);

                switch (prop.PropertyType.Name)
                {
                    case nameof(Double):
                        foreach (object attr in attrs)
                        {
                            RecipeDescriptionAttribute descriptionAttr = attr as RecipeDescriptionAttribute;
                            if (descriptionAttr != null)
                            {
                                SingleRecipe singleRecipe = new SingleRecipe
                                {
                                    Description = descriptionAttr.Description,
                                    TargetAxis = descriptionAttr.Axis != null ? descriptionAttr.Axis : descriptionAttr.Detail,
                                    Unit = descriptionAttr.Unit,
                                    Motions = CDef.AllAxis.AxisList,
                                    Index = ++Index,
                                    DataUpdateCommand = (this.DataContext as RecipeViewModel).DataUpdateCommand
                                };

                                Binding binding = new Binding(prop.Name)
                                {
                                    Source = recipe,
                                    Mode = BindingMode.TwoWay
                                };
                                singleRecipe.SetBinding(SingleRecipe.ValueProperty, binding);

                                mainPanel.Children.Add(singleRecipe);
                            }
                        }
                        break;
                    case nameof(Boolean):
                        foreach (object attr in attrs)
                        {
                            RecipeDescriptionAttribute descriptionAttr = attr as RecipeDescriptionAttribute;
                            if (descriptionAttr != null)
                            {
                                CheckBox checkBox = new CheckBox
                                {
                                    Content = descriptionAttr.Description,
                                    Tag = descriptionAttr.ExtraDescription,
                                };

                                Binding binding = new Binding(prop.Name)
                                {
                                    Source = recipe,
                                    Mode = BindingMode.TwoWay
                                };
                                checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
                                checkBox.Click += (sender, o) => { (this.DataContext as RecipeViewModel).SaveRecipeCommand.Execute(checkBox); };

                                optionPanel.Children.Add(checkBox);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void RecipeView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllRecipe();
            this.Loaded -= RecipeView_Loaded;  // Prevent RecipePanel Load repeatly
        }
    }
}
