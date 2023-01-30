using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TopCom;
using TopCom.Command;
using TopCom.Models;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for SingleRecipe.xaml
    /// </summary>
    public partial class SingleRecipe : UserControl
    {
        #region Dependency Properties
        public bool IsHeader
        {
            get { return (bool)GetValue(IsHeaderProperty); }
            set { SetValue(IsHeaderProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHeaderProperty =
            DependencyProperty.Register("IsHeader", typeof(bool), typeof(SingleRecipe), new PropertyMetadata(false));

        public bool IsNullSpace
        {
            get { return (bool)GetValue(IsNullSpaceProperty); }
            set { SetValue(IsNullSpaceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsNullSpace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNullSpaceProperty =
            DependencyProperty.Register("IsNullSpace", typeof(bool), typeof(SingleRecipe), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(SingleRecipe), new PropertyMetadata(0));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SingleRecipe), new PropertyMetadata(0.0));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SingleRecipe), new PropertyMetadata(""));

        public string TargetAxis
        {
            get { return (string)GetValue(TargetAxisProperty); }
            set { SetValue(TargetAxisProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TargetAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetAxisProperty =
            DependencyProperty.Register("TargetAxis", typeof(string), typeof(SingleRecipe), new PropertyMetadata(""));

        public ObservableCollection<IMotion> Motions
        {
            get { return (ObservableCollection<IMotion>)GetValue(MotionsProperty); }
            set { SetValue(MotionsProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Motions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MotionsProperty =
            DependencyProperty.Register("Motions", typeof(ObservableCollection<IMotion>), typeof(SingleRecipe), new PropertyMetadata(null));

        public RelayCommand<PositionData> DataUpdateCommand
        {
            get { return (RelayCommand<PositionData>)GetValue(DataUpdateCommandProperty); }
            set { SetValue(DataUpdateCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ButtonCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataUpdateCommandProperty =
            DependencyProperty.Register("DataUpdateCommand", typeof(RelayCommand<PositionData>), typeof(SingleRecipe), new PropertyMetadata(null));

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(SingleRecipe), new PropertyMetadata("mm"));
        #endregion

        public SingleRecipe()
        {
            InitializeComponent();
        }

        private void LabelValue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsNullSpace || IsHeader)
            {
                return;
            }

            PositionData positionData = new PositionData()
            {
                AxisName = TargetAxis,
                OldValue = this.Value,
                Value = this.Value,
                PositionName = Description
            };

            if (Motions != null)
            {
                if (Motions.Any(motion => motion.AxisName == TargetAxis))
                {
                    positionData.CurrentValue = Motions.First(motion => motion.AxisName == TargetAxis).Status.ActualPosition;
                }
                else
                {
                    positionData.CurrentValue = this.Value;
                }
            }

            if (DataUpdateCommand.CanExecute(positionData) == false)
            {
                return;
            }

            ValueEditor valueEditor = new ValueEditor(positionData);
            if (valueEditor.ShowDialog() == true)
            {
                this.Value = positionData.Value;
                if (DataUpdateCommand != null)
                {
                    if (DataUpdateCommand.CanExecute(positionData))
                    {
                        DataUpdateCommand.Execute(positionData);
                    }
                }
            }
        }
    }
}
