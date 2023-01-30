using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopCom.Models
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class RecipeDescriptionAttribute : System.Attribute
    {
        public int Index = 0;
        public string Description;
        public string ExtraDescription;
        public string _Axis;
        public string Axis
        {
            get { return _Axis; }
            set
            {
                if (value != _Axis)
                {
                    _Axis = value;

                    if (Axis != null && Detail != null)
                    {
                        throw (new Exception("Do not set both Axis and Detail on same recipe value." +
                                            "\nRecipe Description: " + Description));
                    }
                }
            }
        }

        /// <summary>
        /// In case Recipe value is not axis target value,
        /// <br/>Detail and Axis should not be define together
        /// <br/>When define Axis, Detail should be keeping null. And otherside.
        /// </summary>
        public string _Detail;
        public string Detail
        {
            get { return _Detail; }
            set
            {
                if (value != _Detail)
                {
                    _Detail = value;

                    if (Axis != null && Detail != null)
                    {
                        throw (new Exception("Do not set both Axis and Detail on same recipe value." +
                                            "\nRecipe Description: " + Description));
                    }
                }
            }
        }
        public string Unit = null;
    }
}
