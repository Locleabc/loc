using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopUI.Define;

namespace TopUI.Models
{
    public interface ITrayCellModel
    {
        //[JsonConverter(typeof(StringEnumConverter))]
        ECellStatus CellStatus { get; set; }
        //[JsonConverter(typeof(StringEnumConverter))]
        ECellShape CellShape { get; set; }
        int CellIndex { get; set; }
        int CellID { get; set; }
    }
}
