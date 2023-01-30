using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TopCom.Models
{
    public delegate void VoidDelegate();
    public delegate bool BooleanDelegate();

    public interface ICylinder
    {
        VoidDelegate MoveForwardHandler { get; set; }
        VoidDelegate MoveBackwardHandler { get; set; }

        BooleanDelegate ConfirmForwardHandler { get; set; }
        BooleanDelegate ConfirmBackwardHandler { get; set; }

        bool IsForward { get; }
        bool IsBackward { get; }

        void MoveForward();
        void MoveBackward();
    }

    public class CCylinder : PropertyChangedNotifier, ICylinder
    {
        public VoidDelegate MoveForwardHandler { get; set; }
        public VoidDelegate MoveBackwardHandler { get; set; }

        public BooleanDelegate ConfirmForwardHandler { get; set; }
        public BooleanDelegate ConfirmBackwardHandler { get; set; }

        protected Timer statusUpdateTimer = new Timer(100);

        public CCylinder()
        {
            statusUpdateTimer.Elapsed += (s, e) =>
            {
                OnPropertyChanged("IsForward");
                OnPropertyChanged("IsBackward");
            };
            statusUpdateTimer.Start();
        }

        public bool IsForward
        {
            get
            {
                if (ConfirmForwardHandler == null)
                {
                    throw new Exception("ConfirmForwardHandler must be assign");
                }

                return ConfirmForwardHandler.Invoke();
            }
        }

        public bool IsBackward
        {
            get
            {
                if (ConfirmBackwardHandler == null)
                {
                    throw new Exception("ConfirmBackwardHandler must be assign");
                }

                return ConfirmBackwardHandler.Invoke();
            }
        }

        public void MoveForward()
        {
            if (MoveForwardHandler == null) throw new Exception("MoveForwardHandler must be assign");

            MoveForwardHandler.Invoke();
        }

        public void MoveBackward()
        {
            if (MoveBackwardHandler == null) throw new Exception("MoveBackwardHandler must be assign");

            MoveBackwardHandler.Invoke();
        }
    }

    public class CPickerCylinder : CCylinder
    {
        public BooleanDelegate ConfirmVacuumHandler { get; set; }

        public VoidDelegate VacuumOnHandler { get; set; }
        public VoidDelegate VacuumOffHandler { get; set; }
        public VoidDelegate PurgeOnHandler { get; set; }
        public VoidDelegate PurgeOffHandler { get; set; }

        public bool IsVacuumOn
        {
            get
            {
                if (ConfirmVacuumHandler == null)
                {
                    throw new Exception("ConfirmVacuumHandler must be assign");
                }

                return ConfirmVacuumHandler.Invoke();
            }
        }

        public void VacuumOn()
        {
            if (VacuumOnHandler == null) throw new Exception("VacuumOnHandler must be assign");

            VacuumOnHandler.Invoke();
        }

        public void VacuumOff()
        {
            if (VacuumOnHandler == null) throw new Exception("VacuumOnHandler must be assign");

            VacuumOffHandler.Invoke();
        }

        public void PurgeOn()
        {
            if (PurgeOnHandler == null) throw new Exception("PurgeOnHandler must be assign");

            PurgeOnHandler.Invoke();
        }

        public void PurgeOff()
        {
            if (PurgeOnHandler == null) throw new Exception("PurgeOnHandler must be assign");

            PurgeOffHandler.Invoke();
        }

        public CPickerCylinder()
            : base()
        {
            statusUpdateTimer.Elapsed += (s, e) =>
            {
                OnPropertyChanged("IsVacuumOn");
            };
        }
    }
}
