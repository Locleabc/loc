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
using PLV_BracketAssemble.Define;
using PLV_BracketAssemble.MVVM.Views;
using PLV_BracketAssemble.Processing;
using TopVision.Lights;
using System.Windows;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class VisionAutoViewModel : PropertyChangedNotifier
    {
        #region Properties
        public IVisionProcess UnderVisionProcess
        {
            get { return _UnderVisionProcess; }
            set
            {
                _UnderVisionProcess = value;
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

        public IVisionResult Result_BottomVision
        {
            get { return _Result_BottomVision; }
            set
            { 
                _Result_BottomVision = value;
                OnPropertyChanged();
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
        #endregion

        #region Commands

        public RelayCommand LiveCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {

                    if (CDef.BotCamera.IsLive)
                    {
                        CDef.BotCamera.Stop();
                        CDef.LightController.SetLightStatus(1, false);

                        OnPropertyChanged("IsUnderVisionLive");
                        return;
                    }

                    CDef.LightController.SetLightStatus(1, true);
                    CDef.LightController.SetLightLevel(1, CDef.UnderVisionRecipe.UnderVision_LightLevel);
                    CDef.BotCamera.Live();

                    OnPropertyChanged("IsUnderVisionLive");
                });
            }
        }

        public RelayCommand GrabCommand_Under
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.LightController.SetLightStatus(1, true);
                    CDef.LightController.SetLightLevel(1, CDef.UnderVisionRecipe.UnderVision_LightLevel);

                    CDef.BotCamera.Grab();

                    Thread.Sleep(100);
                    CDef.LightController.SetLightStatus(1, false);

                    OnPropertyChanged("IsUnderVisionLive");
                },
                (obj) =>
                {
                    return !IsUnderVisionLive;
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
                        FileWriter.WriteAllText(CDef.CurrentRecipeUnderVisionConfigFilePath, JsonConvert.SerializeObject(UnderVisionProcess, Formatting.Indented));

                        CDef.MessageViewModel.Show("Vision parameter saved successed!");
                    }
                    catch (Exception ex)
                    {
                        CDef.MessageViewModel.Show($"Vision parameter save fail!\n{ex.Message}");
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
                    BotVisionRun();
                });
            }
        }

        public RelayCommand TeachCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.MainViewModel.MainContentVM.VisionTeachingVM.ParentVM = this;
                    CDef.MainViewModel.MainContentVM.VisionTeachingVM.VisionProcess = UnderVisionProcess;
                    CDef.MainViewModel.MainContentVM.VisionTeachingVM.VisionProcessVM.DisplayImage = UnderVisionProcess.InputMat;
                    CDef.MainViewModel.MainContentVM.ShowVisionTeachingTab();
                });
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Call on InitViewModel
        /// </summary>
        public void VisionAutoViewModelInit()
        {
            InitVisionProcess();
            CDef.BotCamera.GrabFinished += BotCamera_GrabFinished;
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

        public void InitVisionProcess()
        {
            InitUnderVisionProcess();
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
                UnderVisionProcess = new SingleTemplateMatching(
                    new SingleTemplateMatchingParameter
                    {
                        ROIs = new ObservableCollection<CRectangle>
                        {
                            new CRectangle(0, 0, 1000, 1000),
                        },
                        TemplateTechingRectangle = new CRectangle (200, 200, 600, 600),
                        Threshold = 0.65,
                    }
                );
                SaveCommand.Execute(null);
            }
            else
            {
                UnderVisionProcess = tmpProcess;
            }

            UnderVisionProcess_AssignEventHandler();
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

        public void UnderVisionProcess_AssignEventHandler()
        {
            UnderVisionProcess.OutputMatGenerated += new EventHandler((s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Result_BottomVision = UnderVisionProcess.Result;
                });

                OutputMat_BOT = UnderVisionProcess.OutputMat;
                Task imageSaveTask = Task.Factory.StartNew(() => ImageSaveHandler(UnderVisionProcess, "UnderVision"));
            });
        }

        public void ImageSaveHandler(IVisionProcess VisionProcess, string processName)
        {
            if (CDef.GlobalRecipe.ImageSave == false) return;

            string strVision = processName;
            string strResult = VisionProcess.Result.Judge == EVisionJudge.OK ? "OK" : "NG";

            // TODO : UPDATE OPTION SAVE INPUTIMAGE
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
            // TODO : UPDATE OPTION SAVE SaveProcessImage
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
            // TODO : UPDATE OPTION SAVE SaveResultImage
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
        #endregion

        #region Methods
        private void BotVisionRun()
        {
            UnderVisionProcess.PixelSize = CDef.UnderVisionRecipe.UnderVision_PixelSize;
            UnderVisionProcess.Run();
        }
        #endregion

        #region Privates
        private const string ImageFilePath = @"D:\TOP\Simulation\Images\Top";
        private IVisionProcess _UnderVisionProcess;
        private IVisionResult _Result_BottomVision;
        private Mat _OutputMat_BOT;
        #endregion
    }
}
