using log4net;
using Newtonsoft.Json;

namespace TopCom.Models
{
    public interface ILoggable
    {
        [JsonIgnore]
        ILog Log { get; }
    }
}
