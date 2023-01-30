using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.Lights
{
    public class LightControllerBase : PropertyChangedNotifier, ILightController
    {
        #region Properties
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfChannel
        {
            get { return _NumberOfChannel; }
            set
            {
                if (_NumberOfChannel == value) return;

                _NumberOfChannel = value;
                OnPropertyChanged();
            }
        }

        public ILog Log { get; internal set; }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public virtual bool Open() { return true; }
        public virtual void Close() { }
        public virtual bool GetLightStatus(int channel)
        {
            return true;
        }
        public virtual int GetLightLevel(int channel)
        {
            return 0;
        }
        public virtual void SetLightLevel(int channel, int value)
        {
        }
        public virtual void SetLightStatus(int channel, bool bOnOff)
        {
        }
        #endregion

        #region Privates
        private int _NumberOfChannel;
        private string _Name;
        #endregion
    }
}
