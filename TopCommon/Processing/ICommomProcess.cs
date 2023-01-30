using TopCom.Models;

namespace TopCom.Processing
{
    public interface ICommomProcess/* : IDisposable*/
    {
        bool IsAlive { get; }
        string ProcessName { get; }
        int IntervalTime { get; }
        /// <summary>
        /// Process Timer
        /// </summary>
        ExecuteTimer PTimer { get; set; }
        log4net.ILog Log { get; }
        CStep Step { get; set; }

        /// <summary>
        /// Be called after calling Constructor
        /// </summary>
        PRtnCode ProcessStart();

        void ForceKillProcess();
    }
}
