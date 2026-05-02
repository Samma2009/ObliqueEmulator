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

            {0x36,NOP},
            {0x37,BRK},

            {0x40,ROR},
            {0x41,RORI8},
            {0x42,POPC},
            {0x43,CLZ},
            {0x44,CTZ},
            {0x45,BTST},
            {0x46,SHL},
            {0x47,SHLI8},
            {0x48,LSR},
            {0x49,LSRI8},
            {0x4A,ASR},
            {0x4B,ASRI8},

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

            {0x70,BEXT},
            {0x71,BDEP},

            {0xA0,LDK},
            {0xA1,STK},
            {0xA2,LD},
            {0xA3,ST},

            {0xA4,LDRGN},

            {0xA5,LDBU},
            {0xA6,LDBS},
            {0xA7,LDHU},
            {0xA8,LDHS},
            {0xA9,STB},
            {0xAA,STH},

            {0xAB,LDRIP},

            {0xC0,SPAWN},
            {0xC1,YIELD},

            {0xCB,EXT},

            {0xD6,CICPY},

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
        static void ADDI32(Register rD, uint imm32) => rD._value = rD + imm32;
        static void ADDI16(Register rD, ushort imm16) => rD._value = rD + imm16;
        static void ADDI8(Register rD, byte imm8) => rD._value = rD + imm8;

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
        static void BGT(sbyte rel8) => Register.IP += Register.STAT.GetBit(0) == 0 && Register.STAT.GetBit(1) == Register.STAT.GetBit(3) ? rel8 : 0;
        static void BLE(sbyte rel8) => Register.IP += Register.STAT.GetBit(0) == 1 || Register.STAT.GetBit(1) != Register.STAT.GetBit(3) ? rel8 : 0;
        static void BGEU(sbyte rel8) => Register.IP += Register.STAT.GetBit(2) == 0 ? rel8 : 0;
        static void BLTU(sbyte rel8) => Register.IP += Register.STAT.GetBit(2) == 1 ? rel8 : 0;
        static void BGTU(sbyte rel8) => Register.IP += Register.STAT.GetBit(2) == 0 && Register.STAT.GetBit(0) == 0 ? rel8 : 0;
        static void BLEU(sbyte rel8) => Register.IP += Register.STAT.GetBit(2) == 1 || Register.STAT.GetBit(0) == 0 ? rel8 : 0;

        static void NOP() { }
        static void BRK() { Program.IsPaused = true; Register.DumpRegister(); }

        static void SPAWN(Register rD, Register rFn, Register rArg) => rD._value = Register.SpawnFiber(rFn,rArg);
        static void YIELD(Register rQ) => Register.SwitchFiber(rQ);
        static void EXT(Register rD, Register rS,byte subop8) => NOP();

        static void LDK(Register rD, Register rA, sbyte off8)
        {
            uint addr = (uint)(rA._value + off8);
            if (Program.Memory.CheckCLT6(addr))
                rD._value = Program.Memory.ReadU32(addr);
            else throw new EmulationException($"Unaligned memory access at address 0x{addr:X8}");

        }
        static void STK(Register rA, sbyte off8, Register rS)
        {
            uint addr = (uint)(rA._value + off8);

            if (Program.Memory.CheckCLT6(addr))
                Program.Memory.WriteU32(addr, rS._value);
            else throw new EmulationException($"Unaligned memory access at address 0x{addr:X8}");
        }

        static void LD(Register rD, Register rA, sbyte off8) => rD._value = Program.Memory.ReadU32((uint)(rA._value + off8));
        static void ST(Register rA, sbyte off8, Register rS) => Program.Memory.WriteU32((uint)(rA._value + off8), rS._value);

        static void LDRIP(Register rD, int rel32) => rD._value = Program.Memory.ReadU32(Register.IP + (uint)rel32);

        static void CICPY(Register rD, Register rS, Register rC)
        {
            uint s = rC._value;

            while (s > 0)
            {
                if (s >= 4) 
                {
                    Program.Memory.WriteU32(rD, Program.Memory.ReadU32(rS));
                    rS += 4;
                    rD += 4;
                    s -= 4;
                }
                else if (s >= 2)
                {
                    Program.Memory.WriteU16(rD, Program.Memory.ReadU16(rS));
                    rS += 2;
                    rD += 2;
                    s -= 2;
                }
                else
                {
                    Program.Memory[rD] = Program.Memory[rS];
                    rS += 1;
                    rD += 1;
                    s -= 1;
                }
            }
        }

        static void LDRGN(Register rD, Register rA, byte len8)
        {
            if (!Program.Memory.IsRegionReadable(rA._value, len8))
            {
                Register.STAT.SetBit(3, true);
                return;
            }

            rD._value = Program.Memory.ReadU32(rA._value);
            rA._value += len8;
        }

        static void LDBU(Register rD, Register rA, sbyte off8) => rD._value = Program.Memory[(uint)(rA._value + off8)];
        static void LDBS(Register rD, Register rA, sbyte off8) => rD._value = (uint)(sbyte)Program.Memory[(uint)(rA._value + off8)];

        static void LDHU(Register rD, Register rA, sbyte off8) => rD._value = Program.Memory.ReadU16((uint)(rA._value + off8));
        static void LDHS(Register rD, Register rA, sbyte off8) => rD._value = (uint)(short)Program.Memory.ReadU16((uint)(rA._value + off8));

        static void STB(Register rA, sbyte off8, Register rS) => Program.Memory[(uint)(rA._value + off8)] = (byte)rS._value;
        static void STH(Register rA, sbyte off8, Register rS) => Program.Memory.WriteU16((uint)(rA._value + off8), (ushort)rS._value);

        static void ROR(Register rD, Register rS) => rD._value = (uint)BitOperations.RotateRight(rD,rS);
        static void RORI8(Register rD, byte imm8) => rD._value = (uint)BitOperations.RotateRight(rD, imm8);

        static void POPC(Register rD, Register rS) => rD._value = (uint)BitOperations.PopCount(rS);
        static void CLZ(Register rD, Register rS) => rD._value = (uint)BitOperations.LeadingZeroCount(rS);
        static void CTZ(Register rD, Register rS) => rD._value = (uint)BitOperations.TrailingZeroCount(rS);

        static void BTST(Register rD, byte imm8) => Register.STAT.SetBit(0,rD.GetBit(imm8) == 1 ? false : true);

        static void SHL(Register rD, Register rS) => rD._value = (uint)(rD << (rS & 31));
        static void SHLI8(Register rD, byte imm8) => rD._value = (uint)(rD << (imm8 & 31));

        static void LSR(Register rD, Register rS) => rD._value = (uint)(rD >> (rS & 31));
        static void LSRI8(Register rD, byte imm8) => rD._value = (uint)(rD >> (imm8 & 31));

        static void ASR(Register rD, Register rS) => rD._value = (uint)((int)rD._value >> (int)(rS._value & 31));

        static void ASRI8(Register rD, byte imm8) => rD._value = (uint)((int)rD._value >> (imm8 & 31));

        static void BEXT(Register rD, Register rS, Register rM)
        {
            uint result = 0;
            uint src = rS._value;
            uint mask = rM._value;
            int outBit = 0;

            for (int i = 0; i < 32; i++)
                if ((mask >> i & 1) == 1) result |= ((src >> i) & 1) << outBit++;

            rD._value = result;
        }
        static void BDEP(Register rD, Register rS, Register rM)
        {
            uint result = 0;
            uint src = rS._value;
            uint mask = rM._value;
            int inBit = 0;

            for (int i = 0; i < 32; i++)
                if ((mask >> i & 1) == 1) result |= ((src >> inBit++) & 1) << i;

            rD._value = result;
        }
    }
}
