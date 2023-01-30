using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopMotion.IO
{
    public enum EInputPin_DIOPlusR : uint
    {
        LimitPositive = 0x00000001,
        LimitNegative = 0x00000002,
        Origin = 0x00000004,
        ClearPosition = 0x00000008,
        PT_A0 = 0x00000010,
        PT_A1 = 0x00000020,
        PT_A2 = 0x00000040,
        PT_A3 = 0x00000080,
        PT_A4 = 0x00000100,
        PT_A5 = 0x00000200, // or UserIN6
        PT_A6 = 0x00000400, // or UserIN7
        PT_A7 = 0x00000800, // or UserIN8
        PT_Start = 0x00001000,
        Stop = 0x00002000,
        JogPositive = 0x00002000,
        JogNegative = 0x00004000,
        AlarmReset = 0x00010000,
        ServoON = 0x00020000,
        Pause = 0x00040000,
        OriginSearch = 0x00080000,
        Teaching = 0x00100000,
        EmgStop = 0x00200000,
        JPT_Input0 = 0x00400000,
        JPT_Input1 = 0x00800000,
        JPT_Input2 = 0x01000000,
        JPT_Start = 0x02000000,
        User_IN0 = 0x04000000,
        User_IN1 = 0x08000000,
        User_IN2 = 0x10000000,
        User_IN3 = 0x20000000,
        User_IN4 = 0x40000000,
        User_IN5 = 0x80000000,
    }

    public enum EOutputPin_DIOPlusR : uint
    {
        CompareOut      = 0x00000001,
        Inposition      = 0x00000002,
        Alarm           = 0x00000004,
        Moving          = 0x00000008,
        Acc_Dec         = 0x00000010,
        ACK             = 0x00000020,
        END             = 0x00000040,
        AlarmBlink      = 0x00000080,
        OriginSearchOK  = 0x00000100,
        ServoReady      = 0x00000200,
        Reserved        = 0x00000400,
        Brake           = 0x00000800,
        PT_Output0      = 0x00001000,
        PT_Output1      = 0x00002000,
        PT_Output2      = 0x00004000,
        User_OUT0       = 0x00008000,
        User_OUT1       = 0x00010000,
        User_OUT2       = 0x00020000,
        User_OUT3       = 0x00040000,
        User_OUT4       = 0x00080000,
        User_OUT5       = 0x00100000,
        User_OUT6       = 0x00200000,
        User_OUT7       = 0x00400000,
        User_OUT8       = 0x00800000,
    }
}
