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
                _ when t == typeof(CTLIdx3) => InferCTLIdx3(ref bitsize),
                _ when t == typeof(BytesSize2) => InferByteSize2(ref bitsize),
                _ when t == typeof(int) => InferInt(ref bitsize),
                _ when t == typeof(uint) => InferUint(ref bitsize),
                _ when t == typeof(ushort) => InferUShort(ref bitsize),
                _ when t == typeof(byte) => InferByte(ref bitsize),
                _ when t == typeof(sbyte) => InferSbyte(ref bitsize),
                _ => throw new EmulationException($"Unsupported parameter type {t.FullName}")
            };
        }

        static CTLIdx3 InferCTLIdx3(ref uint bitsize)
        {
            byte value = (byte)(Program.Memory[Register.IP + (bitsize / 8)] & 0xE0);
            var nv = value >> 5;

            bitsize += 3;
            return (CTLIdx3)nv;
        }

        static BytesSize2 InferByteSize2(ref uint bitsize)
        {
            byte value = (byte)(Program.Memory[Register.IP + (bitsize / 8)] & 0xC0);
            var nv = value >> 6;

            bitsize += 2;
            return (BytesSize2)nv;
        }

        static byte InferByte(ref uint bitsize)
        {
            if (bitsize % 8 != 0) bitsize += 8 - (bitsize % 8);
            byte value = Program.Memory[Register.IP + (bitsize / 8)];
            bitsize += 8;
            return value;
        }

        static sbyte InferSbyte(ref uint bitsize)
        {
            if (bitsize % 8 != 0) bitsize += 8 - (bitsize % 8);
            sbyte value = (sbyte)Program.Memory[Register.IP + (bitsize / 8)];
            bitsize += 8;
            return value;
        }

        static ushort InferUShort(ref uint bitsize)
        {
            if (bitsize % 8 != 0) bitsize += 8 - (bitsize % 8);
            uint start = Register.IP + (int)(bitsize / 8);
            bitsize += 8 * 2;

            return Program.Memory.ReadU16(start);
        }

        static uint InferUint(ref uint bitsize)
        {
            if (bitsize % 8 != 0) bitsize += 8 - (bitsize % 8);
            uint start = Register.IP + (int)(bitsize / 8);
            bitsize += 8 * 4;

            var ui = Program.Memory.ReadU32(start);

            return ui;
        }
        static int InferInt(ref uint bitsize)
        {
            if (bitsize % 8 != 0) bitsize += 8 - (bitsize % 8);
            uint start = Register.IP + (int)(bitsize / 8);
            bitsize += 8 * 4;
            return (int)Program.Memory.ReadU32(start);
        }
    }
}
