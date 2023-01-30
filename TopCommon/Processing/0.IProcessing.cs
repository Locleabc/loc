using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TopCom.Processing
{
    public interface IProcessing : ICommomProcess
    {
        IProcessing Parent { get; }
        int MessageCode { get; }
        ProcessingMode Mode { get; }
        string ModeDetail { get; set; }
        EProcessingStatus ProcessingStatus { get; set; }

        ObservableCollection<IProcessing> Childs { get; set; }

        /// <summary>
        /// The main loop of the process
        /// </summary>
        //int ExecuteProcess();
        /// <summary>
        /// Execute right after single circle of main loop start
        /// </summary>
        PRtnCode PreProcess();
        /// <summary>
        /// Execute right before single circle of main loop end
        /// </summary>
        PRtnCode PostProcess();
        PRtnCode ProcessNone();
        PRtnCode ProcessToAlarm();
        PRtnCode ProcessAlarm();
        PRtnCode ProcessToOrigin();
        PRtnCode ProcessOrigin();
        PRtnCode ProcessToStop();
        PRtnCode ProcessStop();
        PRtnCode ProcessToWarning();
        PRtnCode ProcessWarning();
        PRtnCode ProcessToRun();
        PRtnCode ProcessRun();

        void Sleep(int Milisecond);
    }
}
