using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom;
using TopCom.Command;
using TopCom.LOG;
using TopVision;
using TopVision.Algorithms;
using TopVision.Grabbers;
using TopVision.Helpers;
using TopVision.Models;
using VCM_PickAndPlace.Define;
using VCM_PickAndPlace.MVVM.Views;
using VCM_PickAndPlace.Processing;

namespace VCM_PickAndPlace.MVVM.ViewModels
{
    public class VisionAutoViewModel : PropertyChangedNotifier
    {
        #region Properties
        public IVisionProcess LoadVisionProcess
        {
            get { return _LoadVisionProcess; }
            set
            {
                _LoadVisionProcess = value;
                OnPropertyChanged();
            }
        }

        public IVisionProcess UnderVisionProcess
        {
            get { return _UnderVisionProcess; }
            set
            {
                _UnderVisionProcess = value;
                OnPropertyChanged();
            }
        }

        public IVisionProcess UnloadVisionProcess
        {
            get { return _UnloadVisionProcess; }
            set
            {
                _UnloadVisionProcess = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Ball inspect vision
        /// </summary>
        public IVisionProcess InspectVisionProcess
        {
            get { return _InspectVisionProcess; }
            set
            {
                _InspectVisionProcess = value;
                OnPropertyChanged();
            }
        }

        public Mat OutputMat_TOP
        {
            get { return _OutputMat_TOP; }
            set
            {
                _OutputMat_TOP = value;
                OnPropertyChanged();
            }
        }

        public Mat OutputMat_BOT
        {
            get { return _OutputMat_BOT; }
            set
            {
                _OutputMat_BOT = value;
                OnPropertyChanged();
            }
        }

        public Mat OutputMat_BASE
        {
            get { return _OutputMat_BASE; }
            set
            {
                _OutputMat_BASE = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadVisionLive
        {
            get
            {
                if (CDef.TopCamera != null)
                {
                    return CDef.TopCamera.IsLive & CDef.TopCamera.GrabTarget == "GrabTop";
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsUnderVisionLive
        {
            get
            {
                if (CDef.BotCamera != null)
                {
                    return CDef.BotCamera.IsLive;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsUnloadVisionLive
        {
            get
            {
                if (CDef.TopCamera != null)
                {
                    return CDef.TopCamera.IsLive & CDef.TopCamera.GrabTarget == "GrabBase";
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand LiveCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    switch ((EVisionArea)o)
                    {
                        case EVisionArea.LOAD:
                            if (CDef.TopCamera.IsLive)
                            {
                                CDef.TopCamera.Stop();

                                if (CDef.TopCamera.GrabTarget == "GrabTop")
                                {
                                    break;
                                }
                            }

                            CDef.TopCamera.SimulationImageDirectory = CSimulation.TopImagePath;
                            CDef.TopCamera.GrabTarget = "GrabTop";
                            CDef.LightController.SetLightLevel(1, CDef.UpperVisionRecipe.LoadVision_LightLevel);
                            CDef.IO.Output.LightUpper = true;
                            CDef.TopCamera.Live();
                            break;
                        case EVisionArea.UNDER:
                            if (CDef.BotCamera.IsLive)
                            {
                                CDef.BotCamera.Stop();
                                break;
                            }

                            CDef.LightController.SetLightLevel(2, CDef.UnderVisionRecipe.UnderVision_LightLevel);
                            CDef.IO.Output.LightUnder = true;
                            CDef.BotCamera.Live();
                            break;
                        case EVisionArea.UNLOAD:
                            if (CDef.TopCamera.IsLive)
                            {
                                CDef.TopCamera.Stop();

                                if (CDef.TopCamera.GrabTarget == "GrabBase")
                                {
                                    break;
                                }
                            }

                            CDef.TopCamera.SimulationImageDirectory = CSimulation.BaseImagePath;
                            CDef.TopCamera.GrabTarget = "GrabBase";
                            CDef.LightController.SetLightLevel(1, CDef.UpperVisionRecipe.UnloadVision_LightLevel);
                            CDef.IO.Output.LightUpper = true;
                            CDef.IO.Output.LightInspect = false;
                            CDef.TopCamera.Live();
                            break;
                        case EVisionArea.INSPECT:
                            if (CDef.TopCamera.IsLive)
                            {
                                CDef.TopCamera.Stop();

                                if (CDef.TopCamera.GrabTarget == "GrabBase")
                                {
                                    break;
                                }
                            }

                            CDef.TopCamera.SimulationImageDirectory = CSimulation.InspectImagePath;
                            CDef.TopCamera.GrabTarget = "GrabBase";
                            CDef.LightController.SetLightLevel(3, CDef.UpperVisionRecipe.Inspect_LightLevel);
                            CDef.IO.Output.LightUpper = false;
                            CDef.IO.Output.LightInspect = true;
                            CDef.TopCamera.Live();
                            break;
                    }

                    OnPropertyChanged("IsLoadVisionLive");
                    OnPropertyChanged("IsUnderVisionLive");
                    OnPropertyChanged("IsUnloadVisionLive");
                });
            }
        }

        public RelayCommand GrabCommand_Load
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.TopCamera.SimulationImageDirectory = CSimulation.TopImagePath;
                    CDef.TopCamera.GrabTarget = "GrabTop";
                    CDef.TopCamera.Grab();

                    OnPropertyChanged("IsLoadVisionLive");
                },
                (obj) =>
                {
                    return !IsLoadVisionLive;
                });
            }
        }

        public RelayCommand GrabCommand_Under
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.BotCamera.Grab();

                    OnPropertyChanged("IsUnderVisionLive");
                },
                (obj) =>
                {
                    return !IsUnderVisionLive;
                });
            }
        }

        public RelayCommand GrabCommand_Unload
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if ((EVisionArea)o == EVisionArea.UNLOAD)
                    {
                        CDef.TopCamera.SimulationImageDirectory = CSimulation.BaseImagePath;
                    }
                    else
                    {
                        CDef.TopCamera.SimulationImageDirectory = CSimulation.InspectImagePath;
                    }
                    CDef.TopCamera.GrabTarget = "GrabBase";
                    CDef.TopCamera.Grab();

                    OnPropertyChanged("IsUnloadVisionLive");
                },
                (obj) =>
                {
                    return !IsUnloadVisionLive;
                });
            }
        }

        public RelayCommand InspectCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    switch ((EVisionArea)o)
                    {
                        case EVisionArea.LOAD:
                            TopVisionRun();
                            break;
                        case EVisionArea.UNDER:
                            BotVisionRun();
                            break;
                        case EVisionArea.UNLOAD:
                            BaseVisionRun();
                            break;
                        case EVisionArea.INSPECT:
                            InspectVisionRun();
                            break;
                    }
                });
            }
        }

        private IVisionProcess GetVisionProcess(EVisionArea area)
        {
            switch ((EVisionArea)area)
            {
                case EVisionArea.LOAD:
                    return this.LoadVisionProcess;
                case EVisionArea.UNDER:
                    return this.UnderVisionProcess;
                case EVisionArea.UNLOAD:
                    return this.UnloadVisionProcess;
                case EVisionArea.INSPECT:
                    return this.InspectVisionProcess;
            }

            return null;
        }

        public RelayCommand TeachCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    // Checking if VisionTeachWindowView window is opened
                    bool windowOpened = System.Windows.Application.Current.Windows.Cast<System.Windows.Window>().Any(x => x.GetType() == typeof(VisionTeachWindowView));
                    System.Windows.Window openedWindow = null;
                    if (windowOpened)
                    {
                        openedWindow = System.Windows.Application.Current.Windows.Cast<System.Windows.Window>().First(x => x.GetType() == typeof(VisionTeachWindowView));
                    }

                    if (windowOpened)
                    {
                        if ((openedWindow.DataContext as VisionTeachWindowViewModel).VisionProcess == GetVisionProcess((EVisionArea)o))
                        {
                            openedWindow.WindowState = System.Windows.WindowState.Normal;

                            openedWindow.Show();
                            openedWindow.Activate();
                            return;
                        }
                        else
                        {
                            openedWindow.Close();
                        }
                    }

                    VisionTeachWindowView visionTeachWindow = new VisionTeachWindowView(this);
                    (visionTeachWindow.DataContext as VisionTeachWindowViewModel).VisionProcess = GetVisionProcess((EVisionArea)o);

                    visionTeachWindow.Show();
                });
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    try
                    {
                        FileWriter.WriteAllText(CDef.CurrentRecipeLoadVisionConfigFilePath, JsonConvert.SerializeObject(LoadVisionProcess, Formatting.Indented));
                        FileWriter.WriteAllText(CDef.CurrentRecipeUnderVisionConfigFilePath, JsonConvert.SerializeObject(UnderVisionProcess, Formatting.Indented));
                        FileWriter.WriteAllText(CDef.CurrentRecipeUnloadVisionConfigFilePath, JsonConvert.SerializeObject(UnloadVisionProcess, Formatting.Indented));
                        FileWriter.WriteAllText(CDef.CurrentRecipeInspectVisionConfigFilePath, JsonConvert.SerializeObject(InspectVisionProcess, Formatting.Indented));

                        CDef.MessageViewModel.Show("Vision parameter saved successed!");
                    }
                    catch (Exception ex)
                    {
                        CDef.MessageViewModel.Show($"Vision parameter save fail!\n{ex.Message}");
                    }
                });
            }
        }

        public EVisionArea UnloadInspectCurrentProcess
        {
            get { return _UnloadInspectCurrentProcess; }
            set
            {
                if (_UnloadInspectCurrentProcess == value) return;

                _UnloadInspectCurrentProcess = value;
                OnPropertyChanged();
                OnPropertyChanged("UnloadInspectCurrentProcessText");
            }
        }

        public string UnloadInspectCurrentProcessText
        {
            get
            {
                if (_UnloadInspectCurrentProcess == EVisionArea.UNLOAD)
                {
                    return "Unload\nVision";
                }
                else if (_UnloadInspectCurrentProcess == EVisionArea.INSPECT)
                {
                    return "Inspect\nVision";
                }
                else
                {
                    return "NULL";
                }
            }
        }

        public RelayCommand UnloadInspectProcessChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (UnloadInspectCurrentProcess == EVisionArea.UNLOAD)
                    {
                        UnloadInspectCurrentProcess = EVisionArea.INSPECT;
                    }
                    else
                    {
                        UnloadInspectCurrentProcess = EVisionArea.UNLOAD;
                    }
                });
            }
        }

        #endregion

        #region Constructors
        public VisionAutoViewModel()
        {
            CDef.MainViewModel.InitVM.InitCompletedEvent += (s, e) =>
            {
                InitVisionProcess();

                CDef.TopCamera.GrabFinished += TopCamera_GrabFinished;

                CDef.BotCamera.GrabFinished += BotCamera_GrabFinished;
            };
        }

        private void BotCamera_GrabFinished(object sender, CGrabResult grabResult)
        {
            ICamera camera = sender as ICamera;
            if (camera == null) return;

            if (grabResult.RtnCode == EGrabRtnCode.OK)
            {
                lock (grabResult.GrabImage)
                {
                    if (grabResult.GrabImage.IsNullOrEmpty())
                    {
                        return;
                    }

                    // Display Grab Image
                    OutputMat_BOT = grabResult.GrabImage.Clone();

                    if (camera.IsLive == false)
                    {
                        // Set input image in grab mode
                        UnderVisionProcess.InputMat = OutputMat_BOT.Clone();
                    }
                }
            }

            grabResult.Dispose();
        }

        private void TopCamera_GrabFinished(object sender, CGrabResult grabResult)
        {
            ICamera camera = sender as ICamera;
            if (camera == null) return;

            if (grabResult.RtnCode == EGrabRtnCode.OK)
            {
                lock (grabResult.GrabImage)
                {
                    if (grabResult.GrabImage.IsNullOrEmpty())
                    {
                        return;
                    }

                    if (camera.GrabTarget == "GrabTop")
                    {
                        if (camera.IsLive == false)
                        {
                            // Set input image in grab mode
                            LoadVisionProcess.InputMat = grabResult.GrabImage.Clone();
                        }

                        // Display Grab Image
                        OutputMat_TOP = grabResult.GrabImage.Clone();
                    }
                    else if (camera.GrabTarget == "GrabBase")
                    {
                        if (camera.IsLive == false)
                        {
                            // Set input image in grab mode
                            UnloadVisionProcess.InputMat = grabResult.GrabImage.Clone();
                            InspectVisionProcess.InputMat = grabResult.GrabImage.Clone();
                        }

                        // Display Grab Image
                        OutputMat_BASE = grabResult.GrabImage.Clone();
                    }
                }
            }

            grabResult.Dispose();
        }

        public void InitVisionProcess()
        {
            InitLoadVisionProcess();
            InitUnderVisionProcess();
            InitUnloadVisionProcess();
            InitInspectVisionProcess();
        }

        private IVisionProcess GetProcessFromJson(string JsonText)
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

        public void ImageSaveHandler(IVisionProcess VisionProcess, string processName)
        {
            if (CDef.GlobalRecipe.ImageSave == false) return;

            string strVision = processName;
            string strResult = VisionProcess.Result.Judge == EVisionJudge.OK ? "OK" : "NG";

            if (CDef.GlobalRecipe.SaveInputImage)
            {
                string filePath = Path.Combine(GlobalFolders.FolderImages,
                                               GlobalFolders.CurrentDate,
                                               strVision,
                                               strResult,
                                               "InputImage",
                                               GlobalFolders.CurrentTime + $"_{VisionProcess.TargetID}" + ".jpg");

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    Cv2.ImWrite(filePath, VisionProcess.InputMat);
                }
                catch (Exception ex)
                {
                    UILog.Error($"Image write failed: {ex.Message}");
                }
            }
            if (CDef.GlobalRecipe.SaveProcessImage)
            {
                string filePath = Path.Combine(GlobalFolders.FolderImages,
                                               GlobalFolders.CurrentDate,
                                               strVision,
                                               strResult,
                                               "ProcessedImage",
                                               GlobalFolders.CurrentTime + $"_{VisionProcess.TargetID}" + ".jpg");

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    Cv2.ImWrite(filePath, VisionProcess.PreProcessedMat);
                }
                catch (Exception ex)
                {
                    UILog.Error($"Image write failed: {ex.Message}");
                }
            }
            if (CDef.GlobalRecipe.SaveResultImage)
            {
                string filePath = Path.Combine(GlobalFolders.FolderImages,
                                               GlobalFolders.CurrentDate,
                                               strVision,
                                               strResult,
                                               "ResultImage",
                                               GlobalFolders.CurrentTime + $"_{VisionProcess.TargetID}" + ".jpg");

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    Cv2.ImWrite(filePath, VisionProcess.OutputMat);
                }
                catch (Exception ex)
                {
                    UILog.Error($"Image write failed: {ex.Message}");
                }
            }
        }
        
        public void LoadVisionProcess_AssignEventHandler()
        {
            LoadVisionProcess.OutputMatGenerated += new EventHandler((s, e) =>
            {
                OutputMat_TOP = LoadVisionProcess.OutputMat;
                Task imageSaveTask = Task.Factory.StartNew(() => ImageSaveHandler(LoadVisionProcess, "LoadVision"));
            });
        }
        
        public void UnloadVisionProcess_AssignEventHandler()
        {
            UnloadVisionProcess.OutputMatGenerated += new EventHandler((s, e) =>
            {
                OutputMat_BASE = UnloadVisionProcess.OutputMat;
                Task imageSaveTask = Task.Factory.StartNew(() => ImageSaveHandler(UnloadVisionProcess, "UnloadVision"));
            });
        }
        
        public void UnderVisionProcess_AssignEventHandler()
        {
            UnderVisionProcess.OutputMatGenerated += new EventHandler((s, e) =>
            {
                OutputMat_BOT = UnderVisionProcess.OutputMat;
                Task imageSaveTask = Task.Factory.StartNew(() => ImageSaveHandler(UnderVisionProcess, "UnderVision"));
            });
        }
        
        public void InspectVisionProcess_AssignEventHandler()
        {
            InspectVisionProcess.OutputMatGenerated += new EventHandler((s, e) =>
            {
                OutputMat_BASE = InspectVisionProcess.OutputMat;
                Task imageSaveTask = Task.Factory.StartNew(() => ImageSaveHandler(InspectVisionProcess, "InspectVision"));
            });
        }

        private void InitLoadVisionProcess()
        {
            IVisionProcess tmpProcess = null;

            try
            {
                tmpProcess = GetProcessFromJson(File.ReadAllText(CDef.CurrentRecipeLoadVisionConfigFilePath));
            }
            catch { }

            if (tmpProcess == null)
            {
                LoadVisionProcess = new MultiTemplateMatching(
                    new MultiTemplateMatchingParameter
                    {
                        ROIs = new ObservableCollection<CRectangle>
                        {
                            new CRectangle(1590, 1390, 400, 400),
                            new CRectangle(478, 1266, 400, 400),
                            new CRectangle(602, 154, 400, 400),
                            new CRectangle(1714, 277, 400, 400),
                        },
                        TemplateImagePath = Path.Combine(CDef.CurrentRecipeFolder, "Vision", "TOP", "MTMParameter", "Pattern.jpg"),
                        TemplateCount = 4,
                        Threshold = 0.65,
                        RefTemplateCount = 3,
                    }
                );
                LoadVisionProcess.SiblingProcessors.Add(
                    new DirectDetection(
                        new DirectDetectionParameter
                        {
                            ROIs = new ObservableCollection<CRectangle> { new CRectangle(1590, 1390, 400, 400) },
                            TemplateImageFolder = Path.Combine(CDef.CurrentRecipeFolder, "Vision", "TOP", "DDParameter", "Pattern.jpg"),
                            MinimalPhaseDiff = 90,
                            Threshold = 0.6,
                            RotateDirection = ERotateDirect.CW,
                        }
                    )
                );

                SaveCommand.Execute(EVisionArea.LOAD);
            }
            else
            {
                LoadVisionProcess = tmpProcess;
            }

            LoadVisionProcess_AssignEventHandler();
        }

        private void InitUnderVisionProcess()
        {
            IVisionProcess tmpProcess = null;

            try
            {
                tmpProcess = GetProcessFromJson(File.ReadAllText(CDef.CurrentRecipeUnderVisionConfigFilePath));
            }
            catch { }

            if (tmpProcess == null)
            {
                UnderVisionProcess = new MultiTemplateMatching(
                    new MultiTemplateMatchingParameter
                    {
                        ROIs = new ObservableCollection<CRectangle>
                        {
                            new CRectangle(510, 210, 600, 600),
                            new CRectangle(1500, 210, 600, 600),
                            new CRectangle(510, 1200, 600, 600),
                            new CRectangle(1500, 1200, 600, 600),
                        },
                        TemplateImagePath = Path.Combine(CDef.CurrentRecipeFolder, "Vision", "BOT", "MTMParameter", "Pattern.jpg"),
                        TemplateCount = 4,
                        Threshold = 0.65,
                        RefTemplateCount = 4,
                    }
                );
                SaveCommand.Execute(EVisionArea.UNDER);
            }
            else
            {
                UnderVisionProcess = tmpProcess;
            }

            UnderVisionProcess_AssignEventHandler();
        }

        private void InitUnloadVisionProcess()
        {
            IVisionProcess tmpProcess = null;

            try
            {
                tmpProcess = GetProcessFromJson(File.ReadAllText(CDef.CurrentRecipeUnloadVisionConfigFilePath));
            }
            catch { }

            if (tmpProcess == null)
            {
                UnloadVisionProcess = new ContourDetection(
                    new ContourDetectionParameter
                    {
                        ROIs = new ObservableCollection<CRectangle>
                        {
                            new CRectangle(1590, 1390, 400, 400),
                            new CRectangle(478, 1266, 400, 400),
                            new CRectangle(602, 154, 400, 400),
                            new CRectangle(1714, 277, 400, 400),
                        },
                        Threshold = 0.65,
                        CoutourArea = 1000,
                    }
                );

                SaveCommand.Execute(EVisionArea.UNLOAD);
            }
            else
            {
                UnloadVisionProcess = tmpProcess;
            }

            UnloadVisionProcess_AssignEventHandler();
        }
        
        private void InitInspectVisionProcess()
        {
            IVisionProcess tmpProcess = null;

            try
            {
                tmpProcess = GetProcessFromJson(File.ReadAllText(CDef.CurrentRecipeInspectVisionConfigFilePath));
            }
            catch { }

            if (tmpProcess == null)
            {
                InspectVisionProcess = new ContourDetection(
                    new ContourDetectionParameter
                    {
                        ROIs = new ObservableCollection<CRectangle>
                        {
                            new CRectangle(1590, 1390, 400, 400),
                            new CRectangle(478, 1266, 400, 400),
                            new CRectangle(602, 154, 400, 400),
                            new CRectangle(1714, 277, 400, 400),
                        },
                        Threshold = 0.65,
                        CoutourArea = 1000,
                    }
                );

                SaveCommand.Execute(EVisionArea.INSPECT);
            }
            else
            {
                InspectVisionProcess = tmpProcess;
            }

            InspectVisionProcess_AssignEventHandler();
        }
        #endregion

        #region Methods
        private void TopVisionRun()
        {
            LoadVisionProcess.PixelSize = CDef.UpperVisionRecipe.LoadVision_PixelSize;
            LoadVisionProcess.Run();
        }

        private void BotVisionRun()
        {
            UnderVisionProcess.PixelSize = CDef.UnderVisionRecipe.UnderVision_PixelSize;
            UnderVisionProcess.Run();
        }

        private void BaseVisionRun()
        {
            UnloadVisionProcess.PixelSize = CDef.UpperVisionRecipe.UnloadVision_PixelSize;
            UnloadVisionProcess.Run();
        }

        private void InspectVisionRun()
        {
            InspectVisionProcess.PixelSize = CDef.UpperVisionRecipe.UnloadVision_PixelSize;
            InspectVisionProcess.Run();
        }
        #endregion

        #region Privates
        private const string ImageFilePath = @"D:\TOP\Simulation\Images\Top";

        private IVisionProcess _LoadVisionProcess;
        private IVisionProcess _UnderVisionProcess;
        private IVisionProcess _UnloadVisionProcess;
        private IVisionProcess _InspectVisionProcess;

        private Mat _OutputMat_TOP;
        private Mat _OutputMat_BOT;
        private Mat _OutputMat_BASE;

        private EVisionArea _UnloadInspectCurrentProcess = EVisionArea.UNLOAD;
        #endregion
    }
}
