using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TopCom;
using TopCom.Command;
using TopVision.Models;
using TopVision.ViewModels;
using PLV_BracketAssemble.Define;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class VisionTeachingViewModel : PropertyChangedNotifier
    {
        #region Properties
        public IVisionProcess VisionProcess
        {
            get { return _VisionProcess; }
            set
            {
                _VisionProcess = value;
                VisionProcessVM.MainProcess = value;

                OnPropertyChanged();
            }
        }

        public VisionAutoViewModel ParentVM
        {
            get { return _ParentViewModel; }
            set
            {
                _ParentViewModel = value;
                OnPropertyChanged();
            }
        }

        public VisionProcessViewModel VisionProcessVM
        {
            get
            {
                return _VisionProcessVM ?? (_VisionProcessVM = new VisionProcessViewModel());
            }
        }
        #endregion

        #region Commands
        public RelayCommand LoadImageCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        VisionProcessVM.MainProcess.InputMat = new Mat(openFileDialog.FileName, ImreadModes.Grayscale);
                        VisionProcessVM.DisplayImage = VisionProcessVM.MainProcess.InputMat;
                    }
                });
            }
        }

        public RelayCommand SaveImageCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    Cv2.ImWrite(Path.Combine(GlobalFolders.FolderImages, $"{DateTime.Now:yyyyMMdd_HHmmss_fff}.jpg")
                                , VisionProcessVM.DisplayImage);
                });
            }
        }

        public RelayCommand InspectCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    ParentVM.InspectCommand.Execute(o);
                });
            }
        }
        #endregion

        public VisionTeachingViewModel()
        {
            VisionProcessVM.OnMainProcessChange = (newProcess, e) =>
            {
                if (newProcess is IVisionProcess)
                {
                    ((IVisionProcess)newProcess).SiblingProcessors = VisionProcess.SiblingProcessors;
                    ((IVisionProcess)newProcess).PreProcessors = VisionProcess.PreProcessors;
                    ((IVisionProcess)newProcess).InputMat = VisionProcess.InputMat?.Clone();

                    ParentVM.UnderVisionProcess = (IVisionProcess)newProcess;
                    VisionProcess = ParentVM.UnderVisionProcess;
                    ParentVM.UnderVisionProcess_AssignEventHandler();

                    VisionProcessVM.MainProcess = VisionProcess;
                }
            };
        }

        #region Privates
        private IVisionProcess _VisionProcess;
        private VisionAutoViewModel _ParentViewModel;
        private VisionProcessViewModel _VisionProcessVM;
        #endregion
    }
}
