using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopCom.Define;

namespace TopCom.Models
{
    public class CStep : PropertyChangedNotifier
    {
        public StepChangeDelegate RunStepChangeHandler { get; set; }
        public StepChangeDelegate HomeStepChangeHandler { get; set; }
        public StepChangeDelegate ToRunStepChangeHandler { get; set; }
        public StepChangeDelegate SubStepChangeHandler { get; set; }

        #region Properties
        public int HomeStep
        {
            get { return _HomeStep; }
            set
            {
                if (value != _HomeStep)
                {
                    _HomeStep = value;
                    HomeStepChangeHandler();
                    OnPropertyChanged();
                }
            }
        }

        public int ToRunStep
        {
            get { return _ToRunStep; }
            set
            {
                int oldValue = _ToRunStep;
                if (value != _ToRunStep)
                {
                    _ToRunStep = value;
                    ToRunStepChangeHandler();
                    OnPropertyChanged();
                }
            }
        }

        public int RunStep
        {
            get { return _RunStep; }
            set
            {
                if (value != _RunStep)
                {
                    _RunStep = value;
                    RunStepChangeHandler();
                    OnPropertyChanged();
                }
            }
        }

        public int SubStep
        {
            get { return _SubStep; }
            set
            {
                if (value != _SubStep)
                {
                    _SubStep = value;
                    SubStepChangeHandler();
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public override string ToString()
        {
            return $"[H]{HomeStep}[TR]{ToRunStep}[R]{RunStep}[SS]{SubStep}";
        }

        #region Privates
        private int _HomeStep;

        private int _ToRunStep;

        private int _RunStep;

        private int _SubStep;

        #endregion

    }
}
