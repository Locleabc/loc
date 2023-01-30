using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Models;

namespace TopVision.Grabbers
{
    public interface ICamera : ILoggable
    {
        #region Properties
        /// <summary>
        /// Identity name of the camera
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Camera current work state
        /// </summary>
        ECameraWorkState WorkState { get; }

        /// <summary>
        /// Is camera thread alive? Set to false to force kill thread
        /// </summary>
        bool IsAlive { get; set; }

        /// <summary>
        /// One camera use on multiple targets
        /// </summary>
        string GrabTarget { get; set; }

        /// <summary>
        /// Grab result (cost and FAIL/OK judge)
        /// </summary>
        CGrabResult GrabResult { get; set; }

        bool IsLive { get; }

        bool IsGrabDone { get; }

        event GrabFinishedHandler GrabFinished;

        double ExposureTime { get; set; }
        string SimulationImageDirectory { get; set; }

        CCameraConfig Config { get; set; }
        #endregion

        #region Methods
        Task<bool> ConnectAsync();
        Task DisconnectAsync();
        void Grab();
        void Live();
        /// <summary>
        /// Live stop
        /// </summary>
        void Stop();
        #endregion
    }
}
