using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TopCom.Models
{
    public class RecipeInfo : PropertyChangedNotifier
    {
        #region Properties
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value == _Name)
                {
                    return;
                }

                _Name = value;
                OnPropertyChanged();
            }
        }

        public string Model
        {
            get
            {
                return _Model;
            }
            set
            {
                if (value == _Model)
                {
                    return;
                }

                _Model = value;
                OnPropertyChanged();
            }
        }

        public string Maker
        {
            get
            {
                return _Maker;
            }
            set
            {
                if (value == _Maker)
                {
                    return;
                }

                _Maker = value;
                OnPropertyChanged();
            }
        }

        public int Index
        {
            get { return _Index; }
            set
            {
                _Index = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private string _Name;
        private string _Model;
        private string _Maker;
        private int _Index;
        #endregion

        #region Constructors
        public RecipeInfo()
            : this("Default")
        {
        }

        public RecipeInfo(string name)
        {
            Name = name;
            Model = "Undefined";
            Maker = "TOP";
        }
        #endregion
    }
}
