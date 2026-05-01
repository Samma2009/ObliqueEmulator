using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Oblique
{
    public class BaseISA : IISAProvider
    {
        public Dictionary<byte, Delegate> InstructionMap { get; set; } = new() 
        {
            {0x00,ADC},
            {0x01,ADCI32},
            {0x02,ADCI16},
            {0x03,ADCI8},

            {0x04,ADD},
            {0x05,ADDI32},
            {0x06,ADDI16},
            {0x07,ADDI8},

            {0x08,AND},
            {0x09,ANDI32},
            {0x0A,ANDI16},
            {0x0B,ANDI8},

            {0x0C,CMP},
            {0x0D,CMPI32},

            {0x0E,SUB},
            {0x0F,OR},
            {0x10,XOR},
            {0x11,NOT},

            {0x12,MUL},
            {0x13,DIV},
            {0x14,UDIV},

            {0x15,MOV},
            {0x16,MOVI32},

            {0x17,ORI32},
            {0x18,ORI16},
            {0x19,ORI8},

            {0x1A,XORI32},
            {0x1B,XORI16},
            {0x1C,XORI8},

            {0x1D,SUBI32},
            {0x1E,SUBI16},
            {0x1F,SUBI8},

            {0x20,BZ},
            {0x21,BNZ},

            {0x22,BS},
            {0x23,BNS},

            {0x24,BO},
            {0x25,BNO},

            {0x26,RET},

            {0x27,J},
            {0x28,J32},

            {0x29,CALL},
            {0x2A,CALLR},

            {0x2B,JR},
            {0x2C,JA},

            {0x2D,CALLA},

            {0x50,ADCF},
            {0x51,ADCI32F},
            {0x52,ADCI16F},
            {0x53,ADCI8F},

            {0x54,ADDF},
            {0x55,ADDI32F},
            {0x56,ADDI16F},
            {0x57,ADDI8F},

            {0x58,ANDF},
            {0x59,ANDI32F},
            {0x5A,ANDI16F},
            {0x5B,ANDI8F},

            {0x5C,SUBF},
            {0x5D,ORF},
            {0x5E,XORF},

            {0x5F,SUBI32F},
            {0x60,SUBI16F},
            {0x61,SUBI8F},

            {0xED,BGE},
            {0xEE,BLT},
            {0xEF,BGT},
            {0xF0,BLE},
            {0xF1,BGEU},
            {0xF2,BLTU},
            {0xF3,BGTU},
            {0xF4,BLEU},
        };

        static void ADC(Register rD, Register rS) => rD._value += rS + Register.STAT.GetBit(2);
        static void ADCI32(Register rD, uint imm32) => rD._value = Register.AddWithCarry(rD, imm32) + Register.STAT.GetBit(2);
        static void ADCI16(Register rD, ushort imm16) => rD._value = Register.AddWithCarry(rD, imm16) + Register.STAT.GetBit(2);
        static void ADCI8(Register rD, byte imm8) => rD._value = Register.AddWithCarry(rD, imm8) + Register.STAT.GetBit(2);

        static void ADD(Register rD, Register rS) => rD._value += rS;
        static void ADDI32(Register rD, uint imm32) => rD._value = Register.AddWithCarry(rD, imm32);
        static void ADDI16(Register rD, ushort imm16) => rD._value = Register.AddWithCarry(rD, imm16);
        static void ADDI8(Register rD, byte imm8) => rD._value = Register.AddWithCarry(rD, imm8);

        static void AND(Register rD, Register rS) => rD._value &= rS;
        static void ANDI32(Register rD, uint imm32) => rD._value &= imm32;
        static void ANDI16(Register rD, ushort imm16) => rD._value &= imm16;
        static void ANDI8(Register rD, byte imm8) => rD._value &= imm8;

        static void CMP(Register rA, Register rB)
        {
            var tmp = rA - rB;
            Register.STAT.SetBit(0,tmp == 0);
            Register.STAT.SetBit(1,tmp < 0);
            Register.STAT.SetBit(2,rA < rB);
            Register.STAT.SetBit(3, ((rA ^ rB) & (rA ^ tmp)) < 0);
            Register.STAT.SetBit(4, (BitOperations.PopCount((uint)tmp & 0xFF) & 1) == 0);
        }
        static void CMPI32(Register rD, uint imm32) => CMP(rD, imm32);

        static void MOV(Register rD, Register rS) => rD._value = rS;
        static void MOVI32(Register rD, uint imm32) => rD._value = imm32;

        static void SUB(Register rD, Register rS) => rD._value -= rS;
        static void OR(Register rD, Register rS) => rD._value |= rS;
        static void XOR(Register rD, Register rS) => rD._value ^= rS;
        static void NOT(Register rD, Register rS) => rD._value = ~rS._value;

        static void MUL(Register rD, Register rA, Register rB) => rD._value = rA * rB;
        static void DIV(Register rD, Register rA, Register rB) => rD._value = (uint)((int)rA / (int)rB);
        static void UDIV(Register rD, Register rA, Register rB) => rD._value = rA / rB;

        static void ORI32(Register rD, uint imm32) => rD._value |= imm32;
        static void ORI16(Register rD, uint imm16) => rD._value |= imm16;
        static void ORI8(Register rD, uint imm8) => rD._value |= imm8;

        static void XORI32(Register rD, uint imm32) => rD._value ^= imm32;
        static void XORI16(Register rD, uint imm16) => rD._value ^= imm16;
        static void XORI8(Register rD, uint imm8) => rD._value ^= imm8;

        static void SUBI32(Register rD, uint imm32) => rD._value -= imm32;
        static void SUBI16(Register rD, uint imm16) => rD._value -= imm16;
        static void SUBI8(Register rD, uint imm8) => rD._value -= imm8;

        static void ADCF(Register rD, Register rS) { ADC(rD, rS); CMP(rD, rS); }
        static void ADCI32F(Register rD, uint imm32) { ADCI32(rD, imm32); CMP(rD, imm32); }
        static void ADCI16F(Register rD, ushort imm16) { ADCI16(rD, imm16); CMP(rD, imm16); }
        static void ADCI8F(Register rD, byte imm8) { ADCI8(rD, imm8); CMP(rD, imm8); }

        static void ADDF(Register rD, Register rS) { ADD(rD, rS); CMP(rD, rS); }
        static void ADDI32F(Register rD, uint imm32) { ADDI32(rD, imm32); CMP(rD, imm32); }
        static void ADDI16F(Register rD, ushort imm16) { ADDI16(rD, imm16); CMP(rD, imm16); }
        static void ADDI8F(Register rD, byte imm8) { ADDI8(rD, imm8); CMP(rD, imm8); }

        static void ANDF(Register rD, Register rS) { AND(rD, rS); CMP(rD, rS); }
        static void ANDI32F(Register rD, uint imm32) { ANDI32(rD, imm32); CMP(rD, imm32); }
        static void ANDI16F(Register rD, ushort imm16) { ANDI16(rD, imm16); CMP(rD, imm16); }
        static void ANDI8F(Register rD, byte imm8) { ANDI8(rD, imm8); CMP(rD, imm8); }

        static void SUBF(Register rD, Register rS) { SUB(rD, rS); CMP(rD, rS); }
        static void SUBI32F(Register rD, uint imm32) { SUBI32(rD, imm32); CMP(rD, imm32); }
        static void SUBI16F(Register rD, ushort imm16) { SUBI16(rD, imm16); CMP(rD, imm16); }
        static void SUBI8F(Register rD, byte imm8) { SUBI8(rD, imm8); CMP(rD, imm8); }

        static void ORF(Register rD, Register rS) { OR(rD, rS); CMP(rD, rS); }
        static void XORF(Register rD, Register rS) { XOR(rD, rS); CMP(rD, rS); }

        static void JR(Register abs32) => Register.IP = abs32._value;
        static void JA(uint abs32) => Register.IP = abs32;
        static void J(sbyte rel8) => Register.IP += rel8;
        static void J32(int rel32) => Register.IP += rel32;

        static void BZ(sbyte rel8) => Register.IP += Register.STAT.GetBit(0) == 1 ? rel8 : 0;
        static void BNZ(sbyte rel8) => Register.IP += Register.STAT.GetBit(0) == 0 ? rel8 : 0;

        static void BS(sbyte rel8) => Register.IP += Register.STAT.GetBit(1) == 1 ? rel8 : 0;
        static void BNS(sbyte rel8) => Register.IP += Register.STAT.GetBit(1) == 0 ? rel8 : 0;

        static void BO(sbyte rel8) => Register.IP += Register.STAT.GetBit(3) == 1 ? rel8 : 0;
        static void BNO(sbyte rel8) => Register.IP += Register.STAT.GetBit(3) == 0 ? rel8 : 0;

        static void RET() => Register.IP = Program.Memory.PopStack();

        static void CALLR(Register abs32)
        {
            Program.Memory.PushStack(Register.IP);
            Register.IP = abs32._value;
        }
        static void CALL(uint rel32)
        {
            Program.Memory.PushStack(Register.IP);
            Register.IP += rel32;
        }
        static void CALLA(uint abs32) => CALLR(abs32);

        static void BGE(sbyte rel8) => Register.IP += Register.STAT.GetBit(1) == Register.STAT.GetBit(3) ? rel8 : 0;
        static void BLT(sbyte rel8) => Register.IP += Register.STAT.GetBit(1) != Register.STAT.GetBit(3) ? rel8 : 0;
        static void BGT(sbyte rel8) => Register.IP += Register.STAT.GetBit(0) == 0 || Register.STAT.GetBit(1) == Register.STAT.GetBit(3) ? rel8 : 0;
        static void BLE(sbyte rel8) => Register.IP += Register.STAT.GetBit(0) == 1 || Register.STAT.GetBit(1) != Register.STAT.GetBit(3) ? rel8 : 0;
        static void BGEU(sbyte rel8) => Register.IP += Register.STAT.GetBit(2) == 0 ? rel8 : 0;
        static void BLTU(sbyte rel8) => Register.IP += Register.STAT.GetBit(2) == 1 ? rel8 : 0;
        static void BGTU(sbyte rel8) => Register.IP += Register.STAT.GetBit(2) == 0 && Register.STAT.GetBit(0) == 0 ? rel8 : 0;
        static void BLEU(sbyte rel8) => Register.IP += Register.STAT.GetBit(2) == 1 || Register.STAT.GetBit(1) == 0 ? rel8 : 0;
    }
}
