using Basler.Pylon;
using log4net;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom.Define;
using TopVision.Helpers;

namespace TopVision.Grabbers
{
    public delegate void GrabFinishedHandler(object sender, CGrabResult grabResult);

    public abstract class CameraBase : ICamera, IDisposable
    {
        #region Properties
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                //OPC();
            }
        }

        public ECameraWorkState WorkState
        {
            get { return _WorkState; }
            internal set
            {
                _WorkState = value;
                //OPC();
            }
        }

        public bool IsAlive
        {
            get { return _IsAlive; }
            set
            {
                _IsAlive = value;
                //OPC();
            }
        }

        public string GrabTarget
        {
            get { return _GrabTarget; }
            set
            {
                _GrabTarget = value;
                //OPC();
            }
        }

        public CGrabResult GrabResult
        {
            get { return _GrabResult; }
            set
            {
                _GrabResult = value;
                //OPC();
            }
        }
        public bool IsLive { get { return WorkState == ECameraWorkState.LIVE; } }
        public bool IsGrabDone { get; private set; }
        public virtual double ExposureTime { get; set; }
        public string SimulationImageDirectory { get; set; }

        public ILog Log { get; internal set; }

        public event GrabFinishedHandler GrabFinished;

        public CCameraConfig Config { get; set; } = new CCameraConfig();

        private int liveCount = 0;
        #endregion

        #region Constructors
        public CameraBase()
        {
            GrabResult = new CGrabResult();

            Log = LogManager.GetLogger($"[CAM]{Name}");
            LogFactory.Configure();

            Init();
        }

        ~CameraBase()
        {
            try
            {
                Disconnect();
            } catch { }
        }

        private void Init()
        {
            IsAlive = true;
            WorkState = ECameraWorkState.NotReady;

            CameraThread = new Thread(OnCameraThread);
            CameraThread.Name = $"{Name}CameraThread";
            CameraThread.IsBackground = true;
            CameraThread.Start();
        }

        private void OnCameraThread()
        {
            while (IsAlive)
            {
                Processing();
                Thread.Sleep(5);
            }
        }

        private void Processing()
        {
            switch (WorkState)
            {
                case ECameraWorkState.ERROR:
                    break;
                case ECameraWorkState.NotReady:
                    break;
                case ECameraWorkState.IDLE:
                    break;
                case ECameraWorkState.GRAB:
                case ECameraWorkState.LIVE:
                    grabWatch.Restart();

#if !SIMULATION
                    bool grabRtn = NativeImageGrab();
#else
                    bool grabRtn = SimulationImageGrab();
#endif

                    grabWatch.Stop();
                    GrabResult.Cost = grabWatch.ElapsedMilliseconds;

                    if (grabRtn == false || GrabResult.GrabImage.IsNullOrEmpty())
                    {
                        Log.Error($"Camera {Name} grab failed!");
                        GrabResult.RtnCode = EGrabRtnCode.GRAB_FAIL;
                        WorkState = ECameraWorkState.ERROR;

                        if (WorkState == ECameraWorkState.GRAB)
                        { Log.Debug($"Grab result: {GrabResult.RtnCode} | Cost: {GrabResult.Cost}ms"); }
                    }
                    else
                    {
                        GrabResult.RtnCode = EGrabRtnCode.OK;
                        
                        // Grab once
                        if (WorkState == ECameraWorkState.GRAB)
                        {
                            // Show log on single image grab only
                            Log.Debug($"Grab result: {GrabResult.RtnCode} | Cost: {GrabResult.Cost}ms");
                            WorkState = ECameraWorkState.IDLE;
                        }
                    }

                    if (WorkState == ECameraWorkState.LIVE)
                    {
                        // Reduce memory / CPU usage
                        Thread.Sleep(100);
                    }

                    liveCount++;
                    if (liveCount >= 5)
                    {
                        liveCount = 0;
                        GC.Collect();
                    }

                    CGrabResult grabResult = new CGrabResult()
                    {
                        Cost = GrabResult.Cost,
                        GrabImage = GrabResult.GrabImage.Clone(),
                        RtnCode = GrabResult.RtnCode,
                    };
                    GrabFinished?.Invoke(this, grabResult);
                    GrabResult.Dispose();
                    grabResult.Dispose();
                    IsGrabDone = true;
                    break;
                default:
                    break;
            }
        }
#endregion

#region Abstracts
        /// <summary>
        /// Real camera connect function
        /// </summary>
        /// <returns></returns>
        internal abstract bool CameraConnect();
        internal abstract bool CameraDisconnect();
        /// <summary>
        /// Set output to GrabImage
        /// </summary>
        /// <returns></returns>
        internal abstract bool NativeImageGrab();
        #endregion

        #region Methods
        public async Task DisconnectAsync()
        {
            await Task.Run(() =>
            {
                Disconnect();
            });
        }

        public async Task<bool> ConnectAsync()
        {
            return await Task<bool>.Run(() =>
            {
                return Connect();
            });
        }

        private bool Connect()
        {
            bool retCode = false;
            try
            {
#if !SIMULATION
                retCode = CameraConnect();
#else
                retCode = SimulationConnect();
#endif
            }
            catch { }
            if (retCode == true)
            {
                WorkState = ECameraWorkState.IDLE;
                Log.Info($"Camera {Name} connected successed...");
                return true;
            }
            else
            {
                WorkState = ECameraWorkState.ERROR;
                Log.Error($"Camera {Name} connect failed...");
                return false;
            }
        }

        private void Disconnect()
        {
#if !SIMULATION
            bool ret = CameraDisconnect();
#else
            bool ret = SimulationDisconnect();
#endif
            if (ret  == false)
            {
                WorkState = ECameraWorkState.NotReady;
            }
            else
            {
                WorkState = ECameraWorkState.ERROR;
            }
        }

        public void Grab()
        {
            IsGrabDone = false;
            if (WorkState == ECameraWorkState.ERROR)
            {
                Log.Error("Camera ERROR STATE. Fix error -> connect and try again.");
                return;
            }

            if (WorkState == ECameraWorkState.NotReady)
            {
                Log.Error("Camera is not connected. Connect camera and try again.");
                return;
            }

            WorkState = ECameraWorkState.GRAB;
        }

        public void Live()
        {
            if (WorkState == ECameraWorkState.ERROR)
            {
                Log.Error("Camera ERROR STATE. Fix error -> connect and try again.");
                return;
            }

            if (WorkState == ECameraWorkState.NotReady)
            {
                Log.Error("Camera is not connected. Connect camera and try again.");
                return;
            }

            WorkState = ECameraWorkState.LIVE;
        }

        public void Stop()
        {
            if (WorkState == ECameraWorkState.LIVE)
            {
                WorkState = ECameraWorkState.IDLE;
            }
        }
#endregion

#region Simulation
        private bool SimulationConnect()
        {
            Log.Debug($"Simulation camera {Name} connected");
            return true;
        }

        private bool SimulationDisconnect()
        {
            Log.Debug($"Simulation camera {Name} disconnected");
            return true;
        }

        private bool SimulationImageGrab()
        {
            if (Directory.Exists(SimulationImageDirectory) == false)
            {
                if (IsLive == false)
                {
                    Log.Warn($"'{SimulationImageDirectory}' folder not exist");
                }
                return false;
            }

            List<string> AllFile = Directory.GetFiles(SimulationImageDirectory, "*.jpg").ToList();

            if (AllFile.Count() <= 0)
            {
                if (IsLive == false)
                {
                    Log.Warn($"No '.jpg' image file in '{SimulationImageDirectory}' folder.");
                }
                return false;
            }

            string randFile = AllFile[new Random().Next(AllFile.Count - 1)];

            if (IsLive == false)
            {
                Log.Debug(randFile);
            }
            GrabResult.GrabImage = new Mat(randFile, ImreadModes.Grayscale);

            return true;
        }

        public void Dispose()
        {
            try
            {
                IsAlive = false;
                if (CameraThread.IsAlive)
                {
                    CameraThread.Abort();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Camera Dispose() error \"{ex.Message}\"");
            }
        }
        #endregion

        #region Privates
        private string _Name;
        private ECameraWorkState _WorkState;
        private bool _IsAlive;
        private string _GrabTarget;
        private CGrabResult _GrabResult;

        Thread CameraThread;
        Stopwatch grabWatch = new Stopwatch();
#endregion
    }
}
