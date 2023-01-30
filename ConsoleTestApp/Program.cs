using Newtonsoft.Json.Linq;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using TopVision;
using TopVision.Grabbers;
using TopVision.Helpers;
using TopVision.Models;
using static OpenCvSharp.Stitcher;

namespace ConsoleTestApp
{
    internal class Program
    {
        #region Defines
        const int headCount = 10;
        #endregion

        static ICamera DemoCamera;
        static System.Timers.Timer GrabTimer;
        static int grabCount = 0;
        static int GrabbedCount
        {
            get
            {
                lock (GrabbedCountLock)
                {
                    return _GrabbedCount;
                }
            }
            set
            {
                lock (GrabbedCountLock)
                {
                    _GrabbedCount = value;
                }
            }
        }
        static int _GrabbedCount;
        static readonly object GrabbedCountLock = new object();

        static readonly Queue<Mat> Mats = new Queue<Mat>();
        static readonly Queue<int> Indexes = new Queue<int>();

        static IVisionProcess VisionProcess;
        static List<IVisionResult> VisionResults;

        static async Task Main(string[] args)
        {
            System.Threading.Thread.Sleep(5000);

            #region 1. PREPARE
            // 1.1. Connect camera
            DemoCamera = new CameraBaslerGigE("TOP");
            await DemoCamera.ConnectAsync();
            DemoCamera.SimulationImageDirectory = @"D:\TOP\Simulation\Images\Base";
            DemoCamera.GrabFinished += Camera_GrabFinished;

            // 1.2. Init Vision Process
            VisionProcess = GetProcessFromJson(File.ReadAllText(@"D:\TOP\TOPVEQ\Recipe\Default\Vision\UNLOAD\UnloadVisionProcess.json"));
            VisionProcess.PixelSize = 4.573;

            // 1.3. Create Vision Process handle Thread
            Thread VisionProcessThread = new Thread(ExecuteProcess)
            {
                IsBackground = true
            };
            VisionProcessThread.Start();
            #endregion

            // 2. Set timer
            GrabTimer = new System.Timers.Timer(100);
            GrabTimer.Elapsed += GrabTimer_Elapsed;
            GrabTimer.AutoReset = true;

        START:
            // 3. Continuous grabbing
            Console.WriteLine($"--------------------------------------------------------------------------------------------");
            grabCount = 0;
            GrabbedCount = 0;
            VisionResults = new List<IVisionResult>();
            GrabTimer.Enabled = true;

            Console.ReadLine();
            goto START;
        }

        private static void ExecuteProcess()
        {
            while (true)
            {
                if (Mats.Count > 0 && VisionProcess.Status != EVisionProcessStatus.IN_PROCESSING)
                {
                    VisionProcess.InputMat = Mats.Dequeue();
                    int index = Indexes.Dequeue();

                    Console.WriteLine($">>>>>> [{DateTime.Now:HH:mm:ss.fff}] Processing #{index} image");

                    VisionProcess.Run();

                    while (VisionProcess.Status != EVisionProcessStatus.PROCESS_DONE)
                    {
                        Thread.Sleep(1);
                    }

                    VisionResults.Add(VisionProcess.Result);

                    Console.WriteLine($">>>>>> [{DateTime.Now:HH:mm:ss.fff}] Processing #{index} image return {VisionProcess.Result}");

                    VisionProcess.InputMat.Dispose();
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        private static void GrabTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            grabCount++;

            Console.WriteLine($"[GRAB] [{DateTime.Now:HH:mm:ss.fff}] The GRAB event #{GrabbedCount + 1} was raised");
            DemoCamera.Grab();

            if (grabCount >= headCount)
            {
                GrabTimer.Enabled = false;
            }
        }

        private static void Camera_GrabFinished(object sender, CGrabResult grabResult)
        {
            Action action = delegate () { QueueingGrabImage(grabResult); };

            Task task = new Task(action);
            task.Start();
        }

        private static void QueueingGrabImage(CGrabResult grabResult)
        {
            if (grabResult.RtnCode == EGrabRtnCode.OK)
            {
                if (grabResult.GrabImage.IsNullOrEmpty())
                {
                    return;
                }

                GrabbedCount++;
                Console.WriteLine($"[GRAB] [{DateTime.Now:HH:mm:ss.fff}] Image #{GrabbedCount} grabbed");

                // Thread.Sleep(200);

                // Add image to queue
                Mats.Enqueue(grabResult.GrabImage.Clone());
                Indexes.Enqueue(GrabbedCount);
            }

            grabResult.Dispose();
        }

        private static IVisionProcess GetProcessFromJson(string JsonText)
        {
            // Create JObject of current VisionProcess
            JObject processObject = JObject.Parse(JsonText);

            // Rescursion condition(s)
            if (processObject == null)
            {
                return null;
            }

            // Get VisionProcess's class FullName 
            Type type = Type.GetType(processObject["ClassFullName"].ToString());

            // Get PreProcessor / SiblingProcessor JToken List before remove from processObject
            IList<JToken> preProcessTokens = processObject["PreProcessors"].Children().ToList();
            IList<JToken> siblingProcessTokens = processObject["SiblingProcessors"].Children().ToList();

            // Only Keep "Parameter" of current VisionProcess,
            // the return VisionProcess will be initialization with Parameter only
            processObject.Remove("ClassFullName");
            processObject.Remove("PreProcessors");
            processObject.Remove("SiblingProcessors");

            IVisionProcess resultProcess = (IVisionProcess)processObject.ToObject(type);

            foreach (JToken token in preProcessTokens)
            {
                resultProcess.PreProcessors.Add(GetProcessFromJson(token.ToString()));
            }

            foreach (JToken token in siblingProcessTokens)
            {
                resultProcess.SiblingProcessors.Add(GetProcessFromJson(token.ToString()));
            }

            return resultProcess;
        }
    }
}
