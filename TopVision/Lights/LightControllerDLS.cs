using log4net;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Define;

namespace TopVision.Lights
{
    public class LightControllerDLS : LightControllerBase
    {
        #region Properties
        public string ComPort
        {
            get { return _ComPort; }
            set
            {
                if (_ComPort == value) return;

                _ComPort = value;
                OnPropertyChanged();
            }
        }

        public int BaudRate
        {
            get { return _BaudRate; }
            set
            {
                if (_BaudRate == value) return;

                _BaudRate = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors & Deconstructors
        public LightControllerDLS(string comPort, int baudRate = 9600, bool withConnect = true)
        {
            Log = LogManager.GetLogger("LIGHT");
            LogFactory.Configure();

            Name = comPort;
            ComPort = comPort;
            BaudRate = baudRate;

#if !SIMULATION
            serialPort = new SerialPort(ComPort, BaudRate, Parity.None, 8, StopBits.One);
            if (withConnect == true)
            {
                Open();
            }
#endif
        }

        ~LightControllerDLS()
        {
            Close();
        }
        #endregion

        #region Methods
        public override bool Open()
        {
            try
            {
#if !SIMULATION
                serialPort.Open();
                Log.Debug($"{ComPort} connected successed!");
#endif

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"{ComPort} connected fail!\n{ex.Message}");

                return false;
            }
        }

        public override void Close()
        {
#if !SIMULATION
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
#endif
        }

        public override void SetLightLevel(int channel, int value)
        {
#if !SIMULATION
            try
            {
                serialPort.Write($"[{channel:00}{value:000}");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
#endif
        }

        public override void SetLightStatus(int channel, bool bOnOff)
        {
#if !SIMULATION
            int value = bOnOff ? 1 : 0;
            serialPort.Write($"]{channel:00}{value}");
#endif
        }
        #endregion

        #region Privates
        private string _ComPort;
        private int _BaudRate;
#if !SIMULATION
        public SerialPort serialPort;
#endif
        #endregion
    }
}
