using log4net;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Models;

namespace TopVision.Models
{
    /// <summary>
    /// Vision Process Description </br>
    /// Run : Process(InputMat + Parameter) -> OutputMat + Result
    /// </summary>
    public interface IVisionProcess : ILoggable
    {
        /// <summary>
        /// Target input image / sample ID
        /// </summary>
        int TargetID { get; set; }
        bool IsPreProcess { get; set; }
        bool IsSiblingProcess { get; set; }
        string DisplayName { get; }
        /// <summary>
        /// Class GetType().FullName
        /// </summary>
        string ClassFullName { get; }
        /// <summary>
        /// Input Image Matrix
        /// </summary>
        Mat InputMat { get; set; }
        /// <summary>
        /// Parameter, should be pass con class constructor
        /// </summary>
        IVisionParameter Parameter { get; set; }
        /// <summary>
        /// Pixel Size in Micrometer
        /// </summary>
        double PixelSize { get; set; }
        /// <summary>
        /// All Image Pre Processing Methods, this all process with be excute in row
        /// </summary>
        ObservableCollection<IVisionProcess> PreProcessors { get; set; }

        ObservableCollection<IVisionProcess> SiblingProcessors { get; set; }
        /// <summary>
        /// Output of all Pre Processing, this one will be input of the main vision processing
        /// </summary>
        Mat PreProcessedMat { get; }

        /// <summary>
        /// Output of the process, can add line, circle, points... result to this Mat to be display
        /// on user interface
        /// </summary>
        Mat OutputMat { get; }
        /// <summary>
        /// Result of the whole process
        /// </summary>
        IVisionResult Result { get; }

        /// <summary>
        /// Status of the whole process
        /// </summary>
        EVisionProcessStatus Status { get; }

        EVisionRtnCode Teaching();

        /// <summary>
        /// Image process function
        /// </summary>
        /// <returns></returns>
        EVisionRtnCode Run(bool isTeachingMode = false);

        /// <summary>
        /// Is Current manual mode
        /// </summary>
        /// <param name="isManual">true: wait preprocessors execute done</param>
        EVisionRtnCode ExcutePreProcessors(bool isManual = false);

        EVisionRtnCode GenerateOutputMat();

        /// <summary>
        /// This event will be invoke after outputMat changed (after whole process)
        /// or whenever OutputMat be updated
        /// </summary>
        event EventHandler OutputMatGenerated;
    }
}
