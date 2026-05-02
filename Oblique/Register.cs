using Gtk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Oblique
{
    public class Register : INumber<Register>
    {
        public uint _value;

        public static uint CurrentFiberHandle = 0;
        public static Dictionary<uint, Fiber> Fibers = new();
        static uint NextStackBase = 0x20000000;

        public static Register[] Bregs 
        {
            get => Fibers[CurrentFiberHandle].Bregs; 
            set => Fibers[CurrentFiberHandle].Bregs = value; 
        }

        public static Register[] CTLregs = new Register[8];

        public static Register IP { get => Fibers[CurrentFiberHandle].IP; set => Fibers[CurrentFiberHandle].IP = value; }
        public static Register STK { get => Fibers[CurrentFiberHandle].STK; set => Fibers[CurrentFiberHandle].STK = value; }
        public static Register FR { get => Fibers[CurrentFiberHandle].FR; set => Fibers[CurrentFiberHandle].FR = value; }
        public static Register STAT { get => Fibers[CurrentFiberHandle].STAT; set => Fibers[CurrentFiberHandle].STAT = value; }

        public static void ResetRegisters()
        {
            Fibers.Clear();
            NextStackBase = 0x20000000;

            CurrentFiberHandle = 0;
            Fibers.Add(CurrentFiberHandle, new(stk: NextStackBase));

            for (int i = 0; i < CTLregs.Length; i++) CTLregs[i] = new();
        }

        public static uint SpawnFiber(uint ip,uint arg = 0)
        {
            uint handle = (uint)Random.Shared.Next(int.MinValue,int.MaxValue);

            while (Fibers.ContainsKey(handle))
                handle = (uint)Random.Shared.Next(int.MinValue, int.MaxValue);

            NextStackBase -= 0x10000;

            var f = new Fiber(ip,NextStackBase);
            f.Bregs[0]._value = arg;

            Fibers.Add(handle, f);

            return handle;
        }

        public static void SwitchFiber(uint handle)
        {
            if (!Fibers.ContainsKey(handle)) throw new EmulationException($"Tried to switch to nonexistant fiber {handle}");
            CurrentFiberHandle = handle;
        }

        public static Register GetBRegisterFromIP(ref uint bitoffset)
        {
            uint byteIndex = IP + bitoffset / 8;
            int lo = Program.Memory[byteIndex];
            int hi = byteIndex + 1 < Program.Memory.Length ? Program.Memory[byteIndex + 1] : 0;

            bitoffset += 4;
            int word = lo | (hi << 8);
            var code = (word >> ((int)bitoffset % 8)) & 0b1111;

            if (code < Bregs.Length) return Bregs[code];
            throw new EmulationException($"Invalid register code {code:X2}");
        }

        public uint GetBit(int bitIndex)
        {
            if (bitIndex < 0 || bitIndex >= 32) throw new EmulationException($"Tried to get bit {bitIndex}");
            return (_value >> bitIndex) & 1;
        }

        public void SetBit(int bitIndex, bool value)
        {
            if (bitIndex < 0 || bitIndex >= 32) throw new EmulationException($"Tried to set bit {bitIndex}");
            if (value) _value |= (1u << bitIndex);
            else _value &= ~(1u << bitIndex);
        }

        public static uint AddWithCarry(uint a, uint b)
        {
            uint result = a + b;
            STAT.SetBit(2, result < a);
            return result;
        }

        public static void DumpRegister() 
        {
            StringBuilder sb = new();

            sb.AppendLine($"Fiber = {CurrentFiberHandle}");
            sb.AppendLine();

            for (int i = 0; i < Bregs.Length; i++)
                sb.AppendLine($"B{i} = 0x{Bregs[i]:X8}");

            sb.AppendLine();

            for (int i = 0; i < CTLregs.Length; i++)
                sb.AppendLine($"CTL{i} = 0x{CTLregs[i]:X8}");

            sb.AppendLine();

            sb.AppendLine($"IP = 0x{IP:X8}");

            Application.Invoke((_, _) =>
            {
                var dialog = new MessageDialog(Program.window, DialogFlags.Modal,
                        MessageType.Error, ButtonsType.Ok, sb.ToString());
                dialog.Run();
                dialog.Destroy();
            });
        }

        public Register() => _value = 0;
        public Register(long value) => _value = (uint)value;
        public Register(int value) => _value = (uint)value;
        public Register(ulong value) => _value = (uint)value;
        public Register(uint value) => _value = value;

        public static Register Zero => new(0);
        public static Register One => new(1);
        public static int Radix => 2;

        public static Register AdditiveIdentity => Zero;
        public static Register MultiplicativeIdentity => One;

        public static Register operator +(Register left, Register right) => new(left._value + right._value);
        public static Register operator -(Register left, Register right) => new(left._value - right._value);
        public static Register operator *(Register left, Register right) => new(left._value * right._value);
        public static Register operator /(Register left, Register right) => new(left._value / right._value);
        public static Register operator %(Register left, Register right) => new(left._value % right._value);
        public static Register operator +(Register value) => value;
        public static Register operator -(Register value) => new(-value._value);
        public static Register operator ++(Register value) => new(value._value + 1);
        public static Register operator --(Register value) => new(value._value - 1);

        public static bool operator ==(Register left, Register right) => left._value == right._value;
        public static bool operator !=(Register left, Register right) => left._value != right._value;
        public static bool operator <(Register left, Register right) => left._value < right._value;
        public static bool operator >(Register left, Register right) => left._value > right._value;
        public static bool operator <=(Register left, Register right) => left._value <= right._value;
        public static bool operator >=(Register left, Register right) => left._value >= right._value;

        public int CompareTo(Register other) => _value.CompareTo(other._value);
        public int CompareTo(object? obj) => obj is Register n ? CompareTo(n) : throw new ArgumentException();
        public bool Equals(Register other) => _value == other._value;
        public override bool Equals(object? obj) => obj is Register n && Equals(n);
        public override int GetHashCode() => _value.GetHashCode();

        public static Register Abs(Register value) => new(Math.Abs(value._value));
        public static bool IsNegative(Register value) => value._value < 0;
        public static Register MaxMagnitude(Register x, Register y) => Abs(x) >= Abs(y) ? x : y;
        public static Register MaxMagnitudeNumber(Register x, Register y) => MaxMagnitude(x, y);
        public static Register MinMagnitude(Register x, Register y) => Abs(x) <= Abs(y) ? x : y;
        public static Register MinMagnitudeNumber(Register x, Register y) => MinMagnitude(x, y);

        public static bool IsCanonical(Register value) => true;
        public static bool IsComplexNumber(Register value) => false;
        public static bool IsEvenInteger(Register value) => uint.IsEvenInteger(value._value) && value._value % 2 == 0;
        public static bool IsFinite(Register value) => double.IsFinite(value._value);
        public static bool IsImaginaryNumber(Register value) => false;
        public static bool IsInfinity(Register value) => double.IsInfinity(value._value);
        public static bool IsInteger(Register value) => double.IsInteger(value._value);
        public static bool IsNaN(Register value) => double.IsNaN(value._value);
        public static bool IsNegativeInfinity(Register value) => double.IsNegativeInfinity(value._value);
        public static bool IsNormal(Register value) => double.IsNormal(value._value);
        public static bool IsOddInteger(Register value) => uint.IsOddInteger(value._value) && value._value % 2 != 0;
        public static bool IsPositive(Register value) => value._value > 0;
        public static bool IsPositiveInfinity(Register value) => double.IsPositiveInfinity(value._value);
        public static bool IsRealNumber(Register value) => true;
        public static bool IsSubnormal(Register value) => double.IsSubnormal(value._value);
        public static bool IsZero(Register value) => value._value == 0;

        public static Register Parse(string s, IFormatProvider? provider)
            => new(int.Parse(s, provider));
        public static Register Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            => new(int.Parse(s, provider));
        public static Register Parse(string s, NumberStyles style, IFormatProvider? provider)
            => new(int.Parse(s, style, provider));
        public static Register Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
            => new(int.Parse(s, style, provider));

        public static bool TryParse(string? s, IFormatProvider? provider, out Register result)
        {
            if (int.TryParse(s, provider, out int d)) { result = new(d); return true; }
            result = Zero; return false;
        }
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Register result)
        {
            if (int.TryParse(s, provider, out int d)) { result = new(d); return true; }
            result = Zero; return false;
        }
        public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Register result)
        {
            if (int.TryParse(s, style, provider, out int d)) { result = new(d); return true; }
            result = Zero; return false;
        }
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Register result)
        {
            if (int.TryParse(s, style, provider, out int d)) { result = new(d); return true; }
            result = Zero; return false;
        }

        public override string ToString() => _value.ToString();
        public string ToString(string? format, IFormatProvider? provider) => _value.ToString(format, provider);
        public bool TryFormat(Span<char> dest, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => _value.TryFormat(dest, out charsWritten, format, provider);

        static bool INumberBase<Register>.TryConvertFromChecked<TOther>(TOther value, out Register result)
        {
            result = new(int.CreateChecked(value));
            return true;
        }
        static bool INumberBase<Register>.TryConvertFromSaturating<TOther>(TOther value, out Register result)
        {
            result = new(int.CreateSaturating(value));
            return true;
        }
        static bool INumberBase<Register>.TryConvertFromTruncating<TOther>(TOther value, out Register result)
        {
            result = new(int.CreateTruncating(value));
            return true;
        }
        static bool INumberBase<Register>.TryConvertToChecked<TOther>(Register value, out TOther result)
        {
            result = TOther.CreateChecked(value._value);
            return true;
        }
        static bool INumberBase<Register>.TryConvertToSaturating<TOther>(Register value, out TOther result)
        {
            result = TOther.CreateSaturating(value._value);
            return true;
        }
        static bool INumberBase<Register>.TryConvertToTruncating<TOther>(Register value, out TOther result)
        {
            result = TOther.CreateTruncating(value._value);
            return true;
        }

        public static implicit operator uint(Register r) => r._value;
        public static implicit operator int (Register r) => (int)r._value;

        public static implicit operator Register(uint r) => new(r);
        public static implicit operator Register(int r) => new(r);

        public static implicit operator Register(ushort r) => new(r);
        public static implicit operator Register(short r) => new(r);

        public static implicit operator Register(byte r) => new(r);
    }
}
