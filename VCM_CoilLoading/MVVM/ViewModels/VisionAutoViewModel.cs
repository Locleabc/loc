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
using TopVision;
using TopVision.Algorithms;
using TopVision.Grabbers;
using TopVision.Models;
using VCM_CoilLoading.Define;
using VCM_CoilLoading.MVVM.Views;
using VCM_CoilLoading.Processing;

namespace VCM_CoilLoading.MVVM.ViewModels
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

        public Mat OutputMat_TOP
        {
            get { return _OutputMat_TOP; }
            set
            {
                _OutputMat_TOP = value;
                OnPropertyChanged();
            }
        }

        public Mat InputMat_TOP
        {
            get { return _InputMat_TOP; }
            set
            {
                _InputMat_TOP = value;
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

        public Mat InputMat_BOT
        {
            get { return _InputMat_BOT; }
            set
            {
                _InputMat_BOT = value;
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

        public Mat InputMat_BASE
        {
            get { return _InputMat_BASE; }
            set
            {
                _InputMat_BASE = value;
                OnPropertyChanged();
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
                            CDef.TopCamera.SimulationImageDirectory = CSimulation.TopImagePath;
                            CDef.IO.Output.LightUpper = true;
                            CDef.TopCamera.Live();
                            break;
                        case EVisionArea.UNDER:
                            CDef.IO.Output.LightUnder = true;
                            CDef.BotCamera.Live();
                            break;
                        case EVisionArea.UNLOAD:
                            CDef.TopCamera.SimulationImageDirectory = CSimulation.BaseImagePath;
                            CDef.IO.Output.LightUpper = true;
                            CDef.TopCamera.Live();
                            break;
                    }
                });
            }
        }

        public RelayCommand GrabCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    switch ((EVisionArea)o)
                    {
                        case EVisionArea.LOAD:
                            CDef.TopCamera.ExposureTime = CDef.UpperVisionRecipe.LoadingVision_ExposureTime;

                            CDef.TopCamera.SimulationImageDirectory = CSimulation.TopImagePath;
                            CDef.TopCamera.GrabTarget = "GrabTop";
                            CDef.TopCamera.Grab();
                            break;
                        case EVisionArea.UNDER:
                            CDef.BotCamera.ExposureTime = CDef.UnderVisionRecipe.UnderVision_ExposureTime;

                            CDef.BotCamera.Grab();
                            break;
                        case EVisionArea.UNLOAD:
                            CDef.TopCamera.ExposureTime = CDef.UpperVisionRecipe.UnloadingVision_ExposureTime;

                            CDef.TopCamera.SimulationImageDirectory = CSimulation.BaseImagePath;
                            CDef.TopCamera.GrabTarget = "GrabBase";
                            CDef.TopCamera.Grab();
                            break;
                    }
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
                            TopVisionInspect();
                            break;
                        case EVisionArea.UNDER:
                            BotVisionInspect();
                            break;
                        case EVisionArea.UNLOAD:
                            BaseVisionInspect();
                            break;
                    }
                });
            }
        }

        public RelayCommand TeachCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    switch ((EVisionArea)o)
                    {
                        case EVisionArea.LOAD:
                            VisionTeachWindowView visionTeachWindow = new VisionTeachWindowView(this);
                            (visionTeachWindow.DataContext as VisionTeachWindowViewModel).VisionProcess = LoadVisionProcess;

                            visionTeachWindow.Show();
                            break;
                        case EVisionArea.UNDER:
                            VisionTeachWindowView botVisionTeachWindow = new VisionTeachWindowView(this);
                            (botVisionTeachWindow.DataContext as VisionTeachWindowViewModel).VisionProcess = UnderVisionProcess;

                            botVisionTeachWindow.Show();
                            break;
                        case EVisionArea.UNLOAD:
                            VisionTeachWindowView baseVisionTeachWindow = new VisionTeachWindowView(this);
                            (baseVisionTeachWindow.DataContext as VisionTeachWindowViewModel).VisionProcess = UnloadVisionProcess;

                            baseVisionTeachWindow.Show();
                            break;
                    }
                });
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    FileWriter.WriteAllText(CDef.CurrentRecipeLoadVisionConfigFilePath, JsonConvert.SerializeObject(LoadVisionProcess, Formatting.Indented));
                    FileWriter.WriteAllText(CDef.CurrentRecipeUnderVisionConfigFilePath, JsonConvert.SerializeObject(UnderVisionProcess, Formatting.Indented));
                    FileWriter.WriteAllText(CDef.CurrentRecipeUnloadVisionConfigFilePath, JsonConvert.SerializeObject(UnloadVisionProcess, Formatting.Indented));
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

                CDef.TopCamera.GrabFinished += (_s, _e) =>
                {
                    if (CDef.TopCamera.GrabResult.RtnCode == EGrabRtnCode.OK)
                    {
                        if (CDef.TopCamera.WorkState == TopVision.Grabbers.ECameraWorkState.LIVE)
                        {
                            if (CDef.TopCamera.GrabTarget == "GrabTop")
                            {
                                // Display Live Image
                                OutputMat_TOP = CDef.TopCamera.GrabImage.Clone();
                            }
                            else if (CDef.TopCamera.GrabTarget == "GrabBase")
                            {
                                // Display Live Image
                                OutputMat_BASE = CDef.TopCamera.GrabImage.Clone();
                            }
                        }
                        else
                        {
                            if (CDef.TopCamera.GrabTarget == "GrabTop")
                            {
                                InputMat_TOP = CDef.TopCamera.GrabImage.Clone();
                                LoadVisionProcess.InputMat = InputMat_TOP;

                                // Display Image after grab
                                OutputMat_TOP = InputMat_TOP.Clone();
                            }
                            else if (CDef.TopCamera.GrabTarget == "GrabBase")
                            {
                                InputMat_BASE = CDef.TopCamera.GrabImage.Clone();
                                UnloadVisionProcess.InputMat = InputMat_BASE;

                                // Display Image after grab
                                OutputMat_BASE = InputMat_BASE.Clone();
                            }
                        }
                    }
                };

                CDef.BotCamera.GrabFinished += (_s, _e) =>
                {
                    if (CDef.BotCamera.GrabResult.RtnCode == EGrabRtnCode.OK)
                    {
                        if (CDef.BotCamera.WorkState == TopVision.Grabbers.ECameraWorkState.LIVE)
                        {
                            // Display Live Image
                            OutputMat_BOT = CDef.TopCamera.GrabImage.Clone();
                        }
                        else
                        {
                            InputMat_BOT = CDef.TopCamera.GrabImage.Clone();
                            UnderVisionProcess.InputMat = InputMat_BOT;

                            // Display Image after grab
                            OutputMat_BOT = InputMat_BOT;
                        }
                    }
                };
            };
        }

        private void InitVisionProcess()
        {
            InitLoadVisionProcess();
            InitUnderVisionProcess();
            InitUnloadVisionProcess();
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

            LoadVisionProcess.OutputMatGenerated += new EventHandler((s, e) =>
            {
                OutputMat_TOP = LoadVisionProcess.OutputMat;
            });
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

            UnderVisionProcess.OutputMatGenerated += new EventHandler((s, e) =>
            {
                OutputMat_BOT = UnderVisionProcess.OutputMat;
            });
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

            UnloadVisionProcess.OutputMatGenerated += new EventHandler((s, e) =>
            {
                OutputMat_BASE = UnloadVisionProcess.OutputMat;
            });
        }
        #endregion

        #region Methods
        private void TopVisionInspect()
        {
            LoadVisionProcess.Run();
        }

        private void BotVisionInspect()
        {
            UnderVisionProcess.Run();
        }

        private void BaseVisionInspect()
        {
            UnloadVisionProcess.Run();
        }
        #endregion

        #region Privates
        private const string ImageFilePath = @"D:\TOP\Simulation\Images\Top";

        private IVisionProcess _LoadVisionProcess;
        private IVisionProcess _UnderVisionProcess;
        private IVisionProcess _UnloadVisionProcess;
        private Mat _OutputMat_TOP;
        private Mat _InputMat_TOP;
        private Mat _OutputMat_BOT;
        private Mat _InputMat_BOT;
        private Mat _OutputMat_BASE;
        private Mat _InputMat_BASE;
        #endregion
    }
}
