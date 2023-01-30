using PLV_BracketAssemble.Define;
using PLV_BracketAssemble.MVVM.ViewModels;
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
using TopMotion;

namespace PLV_BracketAssemble.MVVM.Views
{
    /// <summary>
    /// Interaction logic for ManualControlMotionView.xaml
    /// </summary>
    public partial class ManualControlMotionView : UserControl
    {
        public ManualControlMotionView()
        {
            InitializeComponent();
        }

        private void ManualButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(this.DataContext as ManualControlMotionViewModel).IsModeJogControl) return;

            if (!CDef.AllAxis.XAxis.Status.IsMotionDone || !CDef.AllAxis.XXAxis.Status.IsMotionDone || !CDef.AllAxis.YAxis.Status.IsMotionDone)
            {
                return;
            }

            if (!(this.DataContext as ManualControlMotionViewModel).PickerCylinder1.IsBackward && !(this.DataContext as ManualControlMotionViewModel).PickerCylinder2.IsBackward)
            {
                CDef.MessageViewModel.Show("Please Both Piker Cylinder Up", caption: "Warning");
                return;
            }

            if (!(this.DataContext as ManualControlMotionViewModel).PickerCylinder1.IsBackward)
            {
                CDef.MessageViewModel.Show("Please Picker Cylinder 1 Up", caption: "Warning");
                return;
            }

            if (!(this.DataContext as ManualControlMotionViewModel).PickerCylinder2.IsBackward)
            {
                CDef.MessageViewModel.Show("Please Picker Cylinder 2 Up", caption: "Warning");
                return;
            }

            switch ((sender as Button).Name)
            {
                case "XAxis_Left":
                    (this.DataContext as ManualControlMotionViewModel).X_Axis.MoveJog((this.DataContext as ManualControlMotionViewModel).XAxisVelocity,false);
                    break;
                case "XAxis_Right":
                    (this.DataContext as ManualControlMotionViewModel).X_Axis.MoveJog((this.DataContext as ManualControlMotionViewModel).XAxisVelocity, true);
                    break;

                case "YAxis_Backward":
                    (this.DataContext as ManualControlMotionViewModel).Y_Axis.MoveJog((this.DataContext as ManualControlMotionViewModel).YAxisVelocity,false);
                    break;
                case "YAxis_Forward":
                    (this.DataContext as ManualControlMotionViewModel).Y_Axis.MoveJog((this.DataContext as ManualControlMotionViewModel).YAxisVelocity, true);
                    break;

                case "XXAxis_Left":
                    (this.DataContext as ManualControlMotionViewModel).XX_Axis.MoveJog((this.DataContext as ManualControlMotionViewModel).XXAxisVelocity,false);
                    break;
                case "XXAxis_Right":
                    (this.DataContext as ManualControlMotionViewModel).XX_Axis.MoveJog((this.DataContext as ManualControlMotionViewModel).XXAxisVelocity, true);
                    break;
            }
        }

        private void ManualButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(this.DataContext as ManualControlMotionViewModel).IsModeJogControl) return;

            switch ((sender as Button).Name)
            {
                case "XAxis_Left":
                    (this.DataContext as ManualControlMotionViewModel).X_Axis.SoftStop();
                    break;
                case "XAxis_Right":
                    (this.DataContext as ManualControlMotionViewModel).X_Axis.SoftStop();
                    break;

                case "YAxis_Backward":
                    (this.DataContext as ManualControlMotionViewModel).Y_Axis.SoftStop();
                    break;
                case "YAxis_Forward":
                    (this.DataContext as ManualControlMotionViewModel).Y_Axis.SoftStop();
                    break;

                case "XXAxis_Left":
                    (this.DataContext as ManualControlMotionViewModel).XX_Axis.SoftStop();
                    break;
                case "XXAxis_Right":
                    (this.DataContext as ManualControlMotionViewModel).XX_Axis.SoftStop();
                    break;
            }
        }
    }
}
