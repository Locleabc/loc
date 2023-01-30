using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopCom.Models
{
    public interface IDataHelper
    {
        string FilePath { get; set; }
        object Data { get; set; }

        void Save();
        void Load<T>();
    }
}
