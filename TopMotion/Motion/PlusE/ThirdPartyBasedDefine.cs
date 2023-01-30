using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASTECH
{
    public partial class EziMOTIONPlusELib
    {
		/// <summary>
		/// One or more error occurs
		/// </summary>
		public const uint FFLAG_ERRORALL = 0x00000001;
		/// <summary>
		/// + direction Limit sensor turns ON
		/// </summary>
		public const uint FFLAG_HWPOSILMT = 0x00000002;
		/// <summary>
		/// - direction Limit sensor turns ON
		/// </summary>
		public const uint FFLAG_HWNEGALMT = 0x00000004;
		/// <summary>
		/// + direction program Limit is exceeded
		/// </summary>
		public const uint FFLAG_SWPOGILMT = 0x00000008;
		/// <summary>
		/// - direction program Limit is exceeded
		/// </summary>
		public const uint FFLAG_SWNEGALMT = 0x00000010;
		public const uint Reserved1 = 0x00000020;
		public const uint Reserved2 = 0x00000040;
		/// <summary>
		/// Position error is higher than "Pos Error Overflow Limit" parameter after position command.
		/// </summary>
		public const uint FFLAG_ERRPOSOVERFLOW = 0x00000080;
		/// <summary>
		/// Alarm occurs when an overcurrent occurs in the motor drive element
		/// </summary>
		public const uint FFLAG_ERROVERCURRENT = 0x00000100;
		/// <summary>
		/// Alarm occurs when the motor speed exceeds 3000 [rpm]
		/// </summary>
		public const uint FFLAG_ERROVERSPEED = 0x00000200;
		/// <summary>
		/// Position error is higher than "Pos Tracking Limit" parameter during position command run
		/// </summary>
		public const uint FFLAG_ERRPOSTRACKING = 0x00000400;
		/// <summary>
		/// Alarm occurs when a load exceeding the maximum torque of the motor is applied for a distance
		/// of more than 5 seconds or more than 10 turns
		/// </summary>
		public const uint FFLAG_ERROVERLOAD = 0x00000800;
		/// <summary>
		/// The internal temperature of the drive exceeds 85°C
		/// </summary>
		public const uint FFLAG_ERROVERHEAT = 0x00001000;
		/// <summary>
		/// A counter electromotive force of the motor exceeds 70V
		/// </summary>
		public const uint FFLAG_ERRBACKEMF = 0x00002000;
		/// <summary>
		/// Alarm occurs when it is the motor voltage error
		/// </summary>
		public const uint FFLAG_ERRMOTORPOWER = 0x00004000;
		/// <summary>
		/// Alarm occurs when it is In-position error
		/// </summary>
		public const uint FFLAG_ERRINPOSITION = 0x00008000;
		/// <summary>
		/// If the motor is in an emergency stop
		/// </summary>
		public const uint FFLAG_EMGSTOP = 0x00010000;
		/// <summary>
		/// If the motor is in an emergency stop
		/// </summary>
		public const uint FFLAG_SLOWSTOP = 0x00020000;
		/// <summary>
		/// During homing operation
		/// </summary>
		public const uint FFLAG_ORIGINRETURNING = 0x00040000;
		/// <summary>
		/// In-position is complete
		/// </summary>
		public const uint FFLAG_INPOSITION = 0x00080000;
		/// <summary>
		/// The motor is under Servo On
		/// </summary>
		public const uint FFLAG_SERVOON = 0x00100000;
		/// <summary>
		/// Alarm Reset has run
		/// </summary>
		public const uint FFLAG_ALARMRESET = 0x00200000;
		/// <summary>
		/// Position Table operation has been finished
		/// </summary>
		public const uint FFLAG_PTSTOPED = 0x00400000;
		/// <summary>
		/// The origin sensor is ON
		/// </summary>
		public const uint FFLAG_ORIGINSENSOR = 0x00800000;
		/// <summary>
		/// In case of z-pulse type operation during homing operaiton
		/// </summary>
		public const uint FFLAG_ZPULSE = 0x01000000;
		/// <summary>
		/// Origin return operation has been finished
		/// </summary>
		public const uint FFLAG_ORIGINRETOK = 0x02000000;
		/// <summary>
		/// Motor operating direction (+ :OFF, - :ON) 
		/// </summary>
		public const uint FFLAG_MOTIONDIR = 0x04000000;
		/// <summary>
		/// The motor is running
		/// </summary>
		public const uint FFLAG_MOTIONING = 0x08000000;
		/// <summary>
		/// The motor in running is stopped by Pause command
		/// </summary>
		public const uint FFLAG_MOTIONPAUSE = 0x10000000;
		/// <summary>
		/// The motor is operating to the acceleration section
		/// </summary>
		public const uint FFLAG_MOTIONACCEL = 0x20000000;
		/// <summary>
		/// The motor is operating to the deceleration section
		/// </summary>
		public const uint FFLAG_MOTIONDECEL = 0x40000000;
		/// <summary>
		/// The motor is operating to the normal speed, not acceleration / deceleration sections
		/// </summary>
		public const uint FFLAG_MOTIONCONST = 0x80000000;
	}
}
