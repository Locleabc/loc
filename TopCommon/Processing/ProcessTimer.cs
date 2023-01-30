using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TopCom.Processing
{
    public class ExecuteTimer : PropertyChangedNotifier
    {
        #region Properties
        /// <summary>
        /// StepTimeoutWatcher will be auto update every Step changed
        /// Check ProcessingBase() constructor for more details
        /// </summary>
        public int StepTimeoutWatcher
        {
            get { return _StepTimeoutWatcher; }
            set
            {
                if (_StepTimeoutWatcher != value)
                {
                    _StepTimeoutWatcher = value;
                    OnPropertyChanged("StepTimeoutWatcher");
                }
            }
        }

        public int DelayTimer
        {
            get { return _DelayTimer; }
            set
            {
                if (_DelayTimer != value)
                {
                    _DelayTimer = value;
                    OnPropertyChanged("DelayTimer");
                }
            }
        }

        public ObservableCollection<int> TactTimeCounter
        {
            get { return _TactTimeCounter; }
            set
            {
                if (_TactTimeCounter != value)
                {
                    _TactTimeCounter = value;
                    OnPropertyChanged("TactTimeCounter");
                }
            }
        }

        /// <summary>
        /// This timer need to be set manually
        /// Usage: To watch run time of multiple step, ...
        /// </summary>
        public int SpareTimer
        {
            get { return _SpareTimer; }
            set
            {
                if (_SpareTimer != value)
                {
                    _SpareTimer = value;
                    OnPropertyChanged("SpareTimer");
                }
            }
        }

        public int Now
        {
            get { return Environment.TickCount; }
        }

        /// <summary>
        /// Lead Time for current step
        /// </summary>
        public int StepLeadTime
        {
            get { return Now - StepTimeoutWatcher; }
        }
        #endregion

        #region Methods
        public void ClearTactTimeCounter()
        {
            TactTimeCounter = new ObservableCollection<int> { Environment.TickCount, Environment.TickCount, Environment.TickCount, Environment.TickCount, Environment.TickCount };
        }
        #endregion

        #region Privates
        private int _StepTimeoutWatcher;
        private int _DelayTimer;
        private ObservableCollection<int> _TactTimeCounter = new ObservableCollection<int> { Environment.TickCount, Environment.TickCount, Environment.TickCount, Environment.TickCount, Environment.TickCount };
        private int _SpareTimer;
        #endregion
    }
}
