using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for RecipeUpdateRecorder.xaml
    /// </summary>
    public partial class EventRecorder : UserControl
    {
        #region Dependency Properties
        public ObservableCollection<CEventRecord> Records
        {
            get { return (ObservableCollection<CEventRecord>)GetValue(RecordsProperty); }
            set { SetValue(RecordsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Records.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecordsProperty =
            DependencyProperty.Register("Records", typeof(ObservableCollection<CEventRecord>), typeof(EventRecorder), new PropertyMetadata(new ObservableCollection<CEventRecord>()));
        #endregion

        public EventRecorder()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
