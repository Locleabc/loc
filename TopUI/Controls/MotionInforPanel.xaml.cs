using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using TopCom;
using TopCom.Command;
using TopCom.Models;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for MotionInforPanel.xaml
    /// </summary>
    public partial class MotionInforPanel : UserControl
    {
        #region Dependency Properties
        public IMotion SelectedAxis
        {
            get { return (IMotion)GetValue(SelectedAxisProperty); }
            set { SetValue(SelectedAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedAxisProperty =
            DependencyProperty.Register("SelectedAxis", typeof(IMotion), typeof(MotionInforPanel), new PropertyMetadata(null));

        public ObservableCollection<IMotion> MotionList
        {
            get { return (ObservableCollection<IMotion>)GetValue(MotionListProperty); }
            set { SetValue(MotionListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MotionList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MotionListProperty =
            DependencyProperty.Register("MotionList", typeof(ObservableCollection<IMotion>), typeof(MotionInforPanel), new PropertyMetadata(null));

        public RelayCommand<PositionData> DataUpdateCommand
        {
            get { return (RelayCommand<PositionData>)GetValue(DataUpdateCommandProperty); }
            set { SetValue(DataUpdateCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ButtonCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataUpdateCommandProperty =
            DependencyProperty.Register("DataUpdateCommand", typeof(RelayCommand<PositionData>), typeof(MotionInforPanel), new PropertyMetadata(null));
        #endregion

        public MotionInforPanel()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid TheGrid = sender as DataGrid;

            if (TheGrid.SelectedCells.Count > 0)
            {
                DataGridCellInfo dgci = TheGrid.SelectedCells[0];
                int columnIndex = dgci.Column.DisplayIndex;
                DataGridRow row = TheGrid.ItemContainerGenerator.ContainerFromItem(dgci.Item) as DataGridRow;
                int rowIndex = row.GetIndex();

                if (rowIndex != 6)
                {
                    return;
                }

                //ValueEditor ve = new ValueEditor(
                //    new TopCom.Models.PositionData
                //    {
                //        AxisName 
                //    }
                //);

            }
        }

        private void DataGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            // iteratively traverse the visual tree
            while ((dep != null) &&
                !(dep is DataGridCell) &&
                !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            if (dep is DataGridCell)
            {
                DataGridCell cell = dep as DataGridCell;
                if (cell.TabIndex == 6)
                {
                    PositionData positionData = new PositionData()
                    {
                        AxisName = SelectedAxis.AxisName,
                        OldValue = SelectedAxis.Speed,
                        Value = SelectedAxis.Speed,
                        PositionName = $"{SelectedAxis.AxisName} Speed [{Unit.mmPerSecond}]"
                    };

                    if (DataUpdateCommand == null) return;
                    if (DataUpdateCommand.CanExecute(positionData) == false) return;

                    ValueEditor valueEditor = new ValueEditor(positionData);
                    if (valueEditor.ShowDialog() == true)
                    {
                        SelectedAxis.Speed = positionData.Value;
                        if (DataUpdateCommand.CanExecute(positionData))
                        {
                            DataUpdateCommand.Execute(positionData);
                        }
                    }
                }
            }
        }
    }
}
