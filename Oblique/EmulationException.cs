using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oblique
{
	public enum EmulationFaultType : uint
	{
		None = 0,
		AlignmentFault = 3,
		IllegalInstruction = 2,
		Breakpoint = 6,
		DivideByZero = 5,
		PageFault = 4,
		ProtectionFault = 1,
		Syscall = 7,
    }

    [Serializable]
	public class EmulationException : Exception
	{
		public EmulationException(EmulationFaultType fault,uint BADADDR, string message) : base(message) 
		{
			Register.CTLregs[7] = (uint)fault;
			Register.BADADDR = BADADDR;

            Program.Memory.PushStack(Register.STAT._value);
            Program.Memory.PushStack(Register.IP._value);

            Register.CTLregs[0] = 0;

            uint vectorAddr = Register.CTLregs[1]._value + (4u * (uint)fault);
            Register.IP._value = Program.Memory.ReadU32(vectorAddr);

            Register.DumpRegister(); Program.IsRunning = false;
        }
    }
}
