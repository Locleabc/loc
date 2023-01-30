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
using VCM_CoilLoading.Define;

namespace VCM_CoilLoading.MVVM.ViewModels
{
    public class VisionTeachWindowViewModel : PropertyChangedNotifier
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
                    Cv2.ImWrite(Path.Combine(ProgramFolder.FolderImages, $"{DateTime.Now:yyyyMMdd_HHmmss_fff}.jpg")
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
                    if (VisionProcess == ParentVM.LoadVisionProcess)
                    {
                        ParentVM.InspectCommand.Execute(EVisionArea.LOAD);
                    }
                    else if (VisionProcess == ParentVM.UnderVisionProcess)
                    {
                        ParentVM.InspectCommand.Execute(EVisionArea.UNDER);
                    }
                });
            }
        }
        #endregion

        public VisionTeachWindowViewModel()
        {
            VisionProcessVM.OnMainProcessChange = (newProcess, e) =>
            {
                if (newProcess is IVisionProcess)
                {
                    if (VisionProcess == ParentVM.LoadVisionProcess)
                    {
                        ParentVM.LoadVisionProcess = (IVisionProcess)newProcess;
                        VisionProcess = ParentVM.LoadVisionProcess;
                    }
                    else if (VisionProcess == ParentVM.UnderVisionProcess)
                    {
                        ParentVM.UnderVisionProcess = (IVisionProcess)newProcess;
                        VisionProcess = ParentVM.UnderVisionProcess;
                    }
                    else if (VisionProcess == ParentVM.UnloadVisionProcess)
                    {
                        ParentVM.UnloadVisionProcess = (IVisionProcess)newProcess;
                        VisionProcess = ParentVM.UnderVisionProcess;
                    }

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
