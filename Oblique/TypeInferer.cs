using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oblique
{
    public static class TypeInferer
    {
        public static object InferParameter(Type t,ref uint bitsize)
        {
            return t switch
            {
                _ when t == typeof(Register) => Register.GetBRegisterFromIP(ref bitsize),
                _ when t == typeof(uint) => InferUint(ref bitsize),
                _ when t == typeof(ushort) => InferUShort(ref bitsize),
                _ when t == typeof(byte) => InferByte(ref bitsize),
                _ => throw new EmulationException($"Unsupported parameter type {t.FullName}")
            };
        }

        static byte InferByte(ref uint bitsize)
        {
            if (bitsize % 8 != 0) bitsize += 8 - (bitsize % 8);
            byte value = Program.Memory[Register.IP + (int)(bitsize / 8)];
            bitsize += 8;
            return value;
        }

        static ushort InferUShort(ref uint bitsize)
        {
            if (bitsize % 8 != 0) bitsize += 8 - (bitsize % 8);
            int start = Register.IP + (int)(bitsize / 8);
            bitsize += 8 * 2;
            return BitConverter.ToUInt16(Program.Memory, start);
        }

        static uint InferUint(ref uint bitsize)
        {
            if (bitsize % 8 != 0) bitsize += 8 - (bitsize % 8);
            int start = Register.IP + (int)(bitsize / 8);
            bitsize += 8 * 4;
            return BitConverter.ToUInt32(Program.Memory, start);
        }
    }
}
