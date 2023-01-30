using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TopCom;
using TopCom.Command;
using TopCom.MES;
using VCM_PickAndPlace.Define;
using VCM_PickAndPlace.MES;

namespace VCM_PickAndPlace.MVVM.ViewModels
{
    public class MESPMStatusViewModel : PropertyChangedNotifier
    {
        #region Properties
        private EPMStatus _ToChangePMStatus = EPMStatus.NONE;
        public EPMStatus ToChangePMStatus
        {
            get { return _ToChangePMStatus; }
            set
            {
                if (_ToChangePMStatus == value) return;

                _ToChangePMStatus = value;
                OnPropertyChanged("LoadDownColor");
                OnPropertyChanged("ModelChangeColor");
                OnPropertyChanged("BMStartColor");
                OnPropertyChanged("MATRDownColor");
            }
        }

        public Brush LoadDownColor
        {
            get
            {
                if (ToChangePMStatus == EPMStatus.LOAD_DOWN)
                {
                    return Brushes.Blue;
                }
                else
                {
                    return new Button().Background;
                }
            }
        }

        public Brush ModelChangeColor
        {
            get
            {
                if (ToChangePMStatus == EPMStatus.MODEL_CHANGE)
                {
                    return Brushes.Blue;
                }
                else
                {
                    return new Button().Background;
                }
            }
        }

        public Brush BMStartColor
        {
            get
            {
                if (ToChangePMStatus == EPMStatus.BM_START)
                {
                    return Brushes.Blue;
                }
                else
                {
                    return new Button().Background;
                }
            }
        }

        public Brush MATRDownColor
        {
            get
            {
                if (ToChangePMStatus == EPMStatus.MATR_DOWN)
                {
                    return Brushes.Blue;
                }
                else
                {
                    return new Button().Background;
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand MESEquipEventChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o.GetType() != typeof(EPMStatus)) return;

                    CDef.MES.Send_PMStatus((EPMStatus)o);
                    ToChangePMStatus = (EPMStatus)o;
                });
            }
        }

        public RelayCommand SavePDCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.MES.SavePD(0);
                    CDef.MES.SavePD(1);

                    CDef.MES.SaveTT(0);
                    CDef.MES.SaveTT(1);
                });
            }
        }
        #endregion
    }
}
