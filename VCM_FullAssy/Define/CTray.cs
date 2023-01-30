using TopUI.Models;

namespace VCM_FullAssy.Define
{
    public class CTray
    {
        public static ITrayModel LoadTray1 { get; set; }
        public static ITrayModel LoadTray2 { get; set; }
        public static ITrayModel LoadTray3 { get; set; }
        public static ITrayModel LoadTray4 { get; set; }

        public static ITrayModel CurrentLoadTray
        {
            get
            {
                if (CurrentLeftLoadTray != null)
                {
                    return CurrentLeftLoadTray;
                }
                else if (CurrentRightLoadTray != null)
                {
                    return CurrentRightLoadTray;
                }
                else
                {
                    return null;
                }
            }
        }
        public static ITrayModel CurrentLeftLoadTray
        {
            get
            {
                if (LoadTray1.WorkIndexInRage)
                {
                    return LoadTray1;
                }
                else if (LoadTray2.WorkIndexInRage)
                {
                    return LoadTray2;
                }
                else
                {
                    // TODO: considering to change this, null value may cause runtime exception
                    return null;
                }
            }
        }
        public static ITrayModel CurrentRightLoadTray
        {
            get
            {
                if (LoadTray3.WorkIndexInRage)
                {
                    return LoadTray3;
                }
                else if (LoadTray4.WorkIndexInRage)
                {
                    return LoadTray4;
                }
                else
                {
                    // TODO: considering to change this, null value may cause runtime exception
                    return null;
                }
            }
        }

        public static ITrayModel UnloadTray1 { get; set; }
        public static ITrayModel UnloadTray2 { get; set; }
        public static ITrayModel UnloadTray3 { get; set; }
        public static ITrayModel UnloadTray4 { get; set; }

        public static ITrayModel CurrentUnloadTray
        {
            get
            {
                if (CurrentLeftUnloadTray != null)
                {
                    return CurrentLeftUnloadTray;
                }
                else if (CurrentRightUnloadTray != null)
                {
                    return CurrentRightUnloadTray;
                }
                else
                {
                    return null;
                }
            }
        }
        public static ITrayModel CurrentLeftUnloadTray
        {
            get
            {
                if (UnloadTray1.WorkIndexInRage)
                {
                    return UnloadTray1;
                }
                else if (UnloadTray2.WorkIndexInRage)
                {
                    return UnloadTray2;
                }
                else
                {
                    // TODO: considering to change this, null value may cause runtime exception
                    return null;
                }
            }
        }
        public static ITrayModel CurrentRightUnloadTray
        {
            get
            {
                if (UnloadTray3.WorkIndexInRage)
                {
                    return UnloadTray3;
                }
                else if (UnloadTray4.WorkIndexInRage)
                {
                    return UnloadTray4;
                }
                else
                {
                    // TODO: considering to change this, null value may cause runtime exception
                    return null;
                }
            }
        }
    }
}
