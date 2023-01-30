using System.Threading;
using TopCom.Models;

namespace TopCom.Processing
{
    public abstract class CommomProcessBase : PropertyChangedNotifier, ICommomProcess
    {
        #region Properties
        public bool IsAlive { get; protected set; } = true;
        public string ProcessName { get; protected set; }
        public int IntervalTime { get; protected set; }
        public ExecuteTimer PTimer
        {
            get { return _PTimer; }
            set
            {
                if (_PTimer != value)
                {
                    _PTimer = value;
                    OnPropertyChanged("PTimer");
                }
            }
        }
        public log4net.ILog Log { get; protected set; }
        public CStep Step
        {
            get
            {
                return _Step;
            }
            set
            {
                if (value != _Step)
                {
                    _Step = value;
                    OnPropertyChanged("Step");
                }
            }
        }
        #endregion

        #region Methods
        public void ForceKillProcess()
        {
            IsAlive = false;
        }

        public PRtnCode ProcessStart()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            IsAlive = true;
            ThisThread = new Thread(ExecuteProcess);
            ThisThread.IsBackground = true;
            ThisThread.Start();

            return nRtn;
        }

        public virtual void ProcessWorker()
        {
            Sleep(10);
        }

        private void ExecuteProcess()
        {
            //Log.Debug($"{ProcessName} Thread started!");
            while (IsAlive)
            {
                ProcessWorker();
                Sleep(IntervalTime);
            }
        }

        /// <summary>
        /// Force Thread to sleep
        /// </summary>
        /// <param name="Milisecond"></param>
        public void Sleep(int Milisecond)
        {
            Thread.Sleep(Milisecond);
        }

        /// <summary>
        /// Sleep only on SIMULATION mode
        /// </summary>
        /// <param name="Milisecond"></param>
        public void SimSleep(int Milisecond)
        {
#if SIMULATION
            Thread.Sleep(Milisecond);
#endif
        }
        #endregion

        #region Privates
        private ExecuteTimer _PTimer = new ExecuteTimer();
        private CStep _Step;

        internal Thread ThisThread;
        #endregion
    }
}
