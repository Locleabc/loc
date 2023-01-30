using Basler.Pylon;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.Grabbers
{
    public class CameraBaslerGigE : CameraBase
    {
        #region Privates
        Camera camera;
        private readonly object grabLock = new object();
        private bool grabDone = false;
        private ManualResetEvent EventGrab = new ManualResetEvent(false);
        PixelDataConverter converter;

        private double _ExposureTime;
        #endregion

        /// <summary>
        /// Exposure Time in percentage
        /// </summary>
        public override double ExposureTime
        {
            get
            {
#if SIMULATION
                return _ExposureTime;
#else
                if (camera == null) return double.NaN;
                if (camera.IsOpen == false) return double.NaN;
                if (camera.IsConnected == false) return double.NaN;

                camera.Parameters[PLCamera.GainAuto].TrySetValue(PLCamera.GainAuto.Off);
                if (camera.GetSfncVersion() < new Version(2, 0, 0)) // Handling for older cameras
                {
                    _ExposureTime = camera.Parameters[PLCamera.ExposureTimeRaw].GetValue();
                }
                else // Handling for newer cameras (using SFNC 2.0, e.g. USB3 Vision cameras)
                {
                    _ExposureTime = camera.Parameters[PLCamera.ExposureTime].GetValue();
                }

                return _ExposureTime;
#endif
            }
            set
            {
#if SIMULATION
                _ExposureTime = value;
#else
                if (value == _ExposureTime) return;
                _ExposureTime = value;

                if (camera == null) return;
                if (camera.IsOpen == false) return;
                if (camera.IsConnected == false) return;

                camera.Parameters[PLCamera.GainAuto].TrySetValue(PLCamera.GainAuto.Off);
                if (camera.GetSfncVersion() < new Version(2, 0, 0)) // Handling for older cameras
                {
                    long min = camera.Parameters[PLCamera.ExposureTimeRaw].GetMinimum();
                    long max = camera.Parameters[PLCamera.ExposureTimeRaw].GetMaximum();
                    long croppedValue = ((long)_ExposureTime / min) * min;

                    if (croppedValue < min) croppedValue = min;
                    if (croppedValue > max) croppedValue = max;

                    camera.Parameters[PLCamera.ExposureTimeRaw].SetValue(croppedValue);
                }
                else // Handling for newer cameras (using SFNC 2.0, e.g. USB3 Vision cameras)
                {
                    double min = camera.Parameters[PLCamera.ExposureTime].GetMinimum();
                    double max = camera.Parameters[PLCamera.ExposureTime].GetMaximum();
                    double croppedValue = ((long)_ExposureTime / min) * min;

                    if (croppedValue < min) croppedValue = min;
                    if (croppedValue > max) croppedValue = max;

                    camera.Parameters[PLCamera.ExposureTime].SetValue(croppedValue);
                }
#endif
            }
        }

        #region Constructors
        public CameraBaslerGigE(string name)
        {
            Name = name;
            converter = new PixelDataConverter();
        }
        #endregion

        #region Overrides
        internal override bool CameraConnect()
        {
            Log.Debug($"Camera {Name} connectting...");
            if (camera == null) camera = new Camera();

            if (camera.IsConnected)
            {
                Log.Debug($"Camera {Name} already connected.");
                CameraDisconnect();
                //return true;
            }

            Log.Debug($"Searching device camera {Name}...");
            if (SearchDevice(Name) == false)
            {
                Log.Error($"Searching device camera {Name} failed...");
                return false;
            }

            Log.Debug($"Opening device camera {Name}...");
            if (OpendAndAddEvent() == false)
            {
                Log.Error($"Opening device camera {Name} failed...");
                return false;
            }

            Log.Debug($"Set configuration device camera {Name}...");
            SetConfiguration();

            return true;
        }

        internal override bool CameraDisconnect()
        {
            if (camera == null) return true;
            //if (camera.IsOpen == false) return true;
            //if (camera.IsConnected == false) return true;

            camera.Close();
            camera.Dispose();
            camera = null;

            Log.Info($"Camera {Name} disconnected.");

            return true;
        }

        internal override bool NativeImageGrab()
        {
            try
            {
                for (int iretry = 0; iretry < 5;)
                {
                    if (camera.WaitForFrameTriggerReady(100, TimeoutHandling.ThrowException))
                    {
                        using (GrabResult.GrabImage)
                        {
                            lock (grabLock)
                            {
                                grabDone = false;
                                EventGrab.Reset();
                                camera.ExecuteSoftwareTrigger();
                                EventGrab.WaitOne(1000);
                            }

                            if (!grabDone)
                            {
                                iretry++;
                                if (iretry < 5)
                                { continue; }
                                else
                                { return false; }
                            }

                            break;
                            // GrabImage will be set in the OnImageGrabbed
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                return false;
            }
        }
        #endregion

        #region Methods
        private bool SearchDevice(string name)
        {
            List<ICameraInfo> allDeviceInfos = new List<ICameraInfo>();
            try
            {
                lock (this)
                {
                    allDeviceInfos = CameraFinder.Enumerate(DeviceType.GigE);

                    if (allDeviceInfos == null || allDeviceInfos.Count <= 0)
                    {
                        Log.Error("No GigE camera found.");
                        return false;
                    }

                    ICameraInfo camInfo = allDeviceInfos.Find(x => x["UserDefinedName"] == name);

                    if (camInfo == null)
                    {
                        Log.Error($"Camera named '{name}' not found!");
                        return false;
                    }
                    else
                    {
                        Log.Debug($"Camera named '{name}' detected success!");
                        camera = new Camera(camInfo);
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private bool OpendAndAddEvent()
        {
            if (camera.IsOpen) { camera.Close(); }

            camera.CameraOpened += Configuration.SoftwareTrigger;
            camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
            camera.ConnectionLost += OnConnectionLost;

            try
            {
                camera.Open(1000, TimeoutHandling.ThrowException);
                Log.Debug($"Camera {Name} open success!");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Camera {Name} open failed \"{ex.Message}\"!");
                return false;
            }
        }

        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            IGrabResult grabResult = e.GrabResult;

            try
            {
                if (grabResult.GrabSucceeded)
                {
                    using (grabResult)
                    {
                        converter.OutputPixelFormat = PixelType.BGR8packed;
                        byte[] buffer = grabResult.PixelData as byte[];
                        using (Mat tmpMat = new Mat(grabResult.Height, grabResult.Width, MatType.CV_8UC1, buffer))
                        {
                            if (GrabResult.GrabImage != null) GrabResult.GrabImage.Dispose();
                            GrabResult.GrabImage = new Mat();
                            Cv2.CvtColor(tmpMat, GrabResult.GrabImage, ColorConversionCodes.BayerBG2GRAY);
                        }
                    }
                }
                else
                {
                    // Clear GrabImage if grabResult FAIL
                    if (GrabResult.GrabImage != null) GrabResult.GrabImage.Dispose();
                    string strLog = "";
                    strLog = string.Format("OnImageGrabbed Error [Code:{0}] / {1}", grabResult.ErrorCode, grabResult.ErrorDescription);
                    Log.Error(strLog);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            // Event grab fail, set grabDone
            // In case grab failed, GrabImage will be disposed so CameraBase class will know that grab is failed
            grabDone = true;
            EventGrab.Set();
        }


        private void OnConnectionLost(object sender, EventArgs e)
        {
            string strLog = "OnConnectionLost has been called.";
            Log.Error(strLog);
            WorkState = ECameraWorkState.NotReady;
        }

        private void SetConfiguration()
        {
            // TODO: Read config file
            string configFile = Path.Combine(GlobalFolders.FolderEQConfig, $"{Name}_CameraConfig.json");
            if (File.Exists(configFile) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configFile));
                File.WriteAllText(configFile, JsonConvert.SerializeObject(Config, Formatting.Indented));
            }

            try
            {
                Config = JsonConvert.DeserializeObject<CCameraConfig>(File.ReadAllText(configFile));

                // Set Packet Size
                // Packet size in bytes on the selected stream channel. Excludes data leader and data trailer.
                camera.Parameters[PLCamera.GevSCPSPacketSize].SetValue(Config.PacketSize);
                // Set Inter-Packet Delay
                // Delay between the transmission of each packet on the selected stream channel.
                camera.Parameters[PLCamera.GevSCPD].SetValue(Config.InterPacketDelay);
            }
            catch (Exception ex)
            {
                Log.Error($"SetConfiguration: {ex.Message}");
            }

            // Make sure to set all parameter before starting StreamGrabber
            camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
        }
        #endregion
    }
}
