using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            {0x2E,LEAVE},
            {0x2F,ENTERI16},

            {0x30,PSTAT},
            {0x31,POPSTAT},
            {0x32,PCTL},
            {0x33,POPCTL},
            {0x34,PFR},
            {0x35,POPFR},

            {0x36,NOP},
            {0x37,BRK},

            {0x38,RDSTK},
            {0x39,WRSTK},
            {0x3A,RDFR},
            {0x3B,WRFR},

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

            {0xD3,RDBAD},

            {0xD6,CICPY},

            {0xE0,SYSRD},
            {0xE1,SYSWR},

            {0xE2,SYSCALL},
            {0xE3,WFI},
            {0xE4,IRET},
            {0xE5,ENI},
            {0xE6,DIS},
            {0xE8,RDTSC},
            {0xE9,CPUID},

            {0xED,BGE},
            {0xEE,BLT},
            {0xEF,BGT},
            {0xF0,BLE},
            {0xF1,BGEU},
            {0xF2,BLTU},
            {0xF3,BGTU},
            {0xF4,BLEU},

            {0xF5,PUSHR},
            {0xF6,POPR},
            {0xF7,PUSHI32},

            {0xFC,SICF},

            {0xFD,OUTPRTS},
            {0xFE,INPRTS},
        };
        public Dictionary<byte, ushort> InstructionSizeMap { get; set; } = new()
        {
            {0x00, 16},
            {0x01, 48},
            {0x02, 32},
            {0x03, 24},
            {0x04, 16},
            {0x05, 48},
            {0x06, 32},
            {0x07, 24},
            {0x08, 16},
            {0x09, 48},
            {0x0A, 32},
            {0x0B, 24},
            {0x0C, 16},
            {0x0D, 48},
            {0x0E, 16},
            {0x0F, 16},
            {0x10, 16},
            {0x11, 16},
            {0x12, 24},
            {0x13, 24},
            {0x14, 24},
            {0x15, 16},
            {0x16, 48},
            {0x17, 48},
            {0x18, 32},
            {0x19, 24},
            {0x1A, 48},
            {0x1B, 32},
            {0x1C, 24},
            {0x1D, 48},
            {0x1E, 32},
            {0x1F, 24},
            {0x20, 16},
            {0x21, 16},
            {0x22, 16},
            {0x23, 16},
            {0x24, 16},
            {0x25, 16},
            {0x26, 8},
            {0x27, 16},
            {0x28, 40},
            {0x29, 40},
            {0x2A, 16},
            {0x2B, 16},
            {0x2C, 40},
            {0x2D, 40},
            {0x2E, 8},
            {0x2F, 24},
            {0x30, 8},
            {0x31, 8},
            {0x32, 16},
            {0x33, 16},
            {0x34, 8},
            {0x35, 8},
            {0x36, 8},
            {0x37, 8},
            {0x38, 16},
            {0x39, 16},
            {0x3A, 16},
            {0x3B, 16},
            {0x40, 16},
            {0x41, 24},
            {0x42, 16},
            {0x43, 16},
            {0x44, 16},
            {0x45, 24},
            {0x46, 16},
            {0x47, 24},
            {0x48, 16},
            {0x49, 24},
            {0x4A, 16},
            {0x4B, 24},
            {0x50, 16},
            {0x51, 48},
            {0x52, 32},
            {0x53, 24},
            {0x54, 16},
            {0x55, 48},
            {0x56, 32},
            {0x57, 24},
            {0x58, 16},
            {0x59, 48},
            {0x5A, 32},
            {0x5B, 24},
            {0x5C, 16},
            {0x5D, 16},
            {0x5E, 16},
            {0x5F, 48},
            {0x60, 32},
            {0x61, 24},
            {0x70, 24},
            {0x71, 24},
            {0x80, 16},
            {0x81, 16},
            {0x82, 16},
            {0x83, 16},
            {0x84, 24},
            {0x85, 16},
            {0x86, 16},
            {0x87, 16},
            {0x88, 48},
            {0x89, 16},
            {0x8A, 16},
            {0x8B, 16},
            {0x8C, 48},
            {0x90, 16},
            {0x91, 16},
            {0x92, 16},
            {0x93, 16},
            {0x94, 16},
            {0x95, 16},
            {0xA0, 24},
            {0xA1, 24},
            {0xA2, 24},
            {0xA3, 24},
            {0xA4, 24},
            {0xA5, 24},
            {0xA6, 24},
            {0xA7, 24},
            {0xA8, 24},
            {0xA9, 24},
            {0xAA, 24},
            {0xAB, 48},
            {0xAC, 24},
            {0xAD, 24},
            {0xB0, 8},
            {0xB1, 8},
            {0xB2, 16},
            {0xC0, 24},
            {0xC1, 16},
            {0xCB, 24},
            {0xD0, 24},
            {0xD1, 24},
            {0xD2, 16},
            {0xD3, 16},
            {0xD6, 24},
            {0xE0, 16},
            {0xE1, 16},
            {0xE2, 8},
            {0xE3, 8},
            {0xE4, 8},
            {0xE5, 8},
            {0xE6, 8},
            {0xE7, 8},
            {0xE8, 16},
            {0xE9, 16},
            {0xEA, 8},
            {0xEB, 8},
            {0xEC, 8},
            {0xED, 16},
            {0xEE, 16},
            {0xEF, 16},
            {0xF0, 16},
            {0xF1, 16},
            {0xF2, 16},
            {0xF3, 16},
            {0xF4, 16},
            {0xF5, 16},
            {0xF6, 16},
            {0xF7, 40},
            {0xF8, 0},
            {0xF9, 0},
            {0xFA, 0},
            {0xFB, 0},
            {0xFC, 16},
            {0xFD, 32},
            {0xFE, 32},
        };
        public Dictionary<byte, string> InstructionAliases { get; set; } = new()
        {
            {0x00, "ADC"},
            {0x01, "ADCI32"},
            {0x02, "ADCI16"},
            {0x03, "ADCI8"},
            {0x04, "ADD"},
            {0x05, "ADDI32"},
            {0x06, "ADDI16"},
            {0x07, "ADDI8"},
            {0x08, "AND"},
            {0x09, "ANDI32"},
            {0x0A, "ANDI16"},
            {0x0B, "ANDI8"},
            {0x0C, "CMP"},
            {0x0D, "CMPI32"},
            {0x0E, "SUB"},
            {0x0F, "OR"},
            {0x10, "XOR"},
            {0x11, "NOT"},
            {0x12, "MUL"},
            {0x13, "DIV"},
            {0x14, "UDIV"},
            {0x15, "MOV"},
            {0x16, "MOVI32"},
            {0x17, "ORI32"},
            {0x18, "ORI16"},
            {0x19, "ORI8"},
            {0x1A, "XORI32"},
            {0x1B, "XORI16"},
            {0x1C, "XORI8"},
            {0x1D, "SUBI32"},
            {0x1E, "SUBI16"},
            {0x1F, "SUBI8"},
            {0x20, "BZ"},
            {0x21, "BNZ"},
            {0x22, "BS"},
            {0x23, "BNS"},
            {0x24, "BO"},
            {0x25, "BNO"},
            {0x26, "RET"},
            {0x27, "J"},
            {0x28, "J32"},
            {0x29, "CALL"},
            {0x2A, "CALLR"},
            {0x2B, "JR"},
            {0x2C, "JA"},
            {0x2D, "CALLA"},
            {0x2E, "LEAVE"},
            {0x2F, "ENTERI16"},
            {0x30, "PSTAT"},
            {0x31, "POPSTAT"},
            {0x32, "PCTL"},
            {0x33, "POPCTL"},
            {0x34, "PFR"},
            {0x35, "POPFR"},
            {0x36, "NOP"},
            {0x37, "BRK"},
            {0x38, "RDSTK"},
            {0x39, "WRSTK"},
            {0x3A, "RDFR"},
            {0x3B, "WRFR"},
            {0x40, "ROR"},
            {0x41, "RORI8"},
            {0x42, "POPC"},
            {0x43, "CLZ"},
            {0x44, "CTZ"},
            {0x45, "BTST"},
            {0x46, "SHL"},
            {0x47, "SHLI8"},
            {0x48, "LSR"},
            {0x49, "LSRI8"},
            {0x4A, "ASR"},
            {0x4B, "ASRI8"},
            {0x50, "ADC.F"},
            {0x51, "ADCI32.F"},
            {0x52, "ADCI16.F"},
            {0x53, "ADCI8.F"},
            {0x54, "ADD.F"},
            {0x55, "ADDI32.F"},
            {0x56, "ADDI16.F"},
            {0x57, "ADDI8.F"},
            {0x58, "AND.F"},
            {0x59, "ANDI32.F"},
            {0x5A, "ANDI16.F"},
            {0x5B, "ANDI8.F"},
            {0x5C, "SUB.F"},
            {0x5D, "OR.F"},
            {0x5E, "XOR.F"},
            {0x5F, "SUBI32.F"},
            {0x60, "SUBI16.F"},
            {0x61, "SUBI8.F"},
            {0x70, "BEXT"},
            {0x71, "BDEP"},
            {0x80, "FADD"},
            {0x81, "FSUB"},
            {0x82, "FMUL"},
            {0x83, "FDIV"},
            {0x84, "FFMA"},
            {0x85, "FINV"},
            {0x86, "FSQRT"},
            {0x87, "FCMP"},
            {0x88, "FCMPI"},
            {0x89, "ITOF"},
            {0x8A, "FTOI"},
            {0x8B, "FMOV"},
            {0x8C, "FMOVI32"},
            {0x90, "FADD.F"},
            {0x91, "FSUB.F"},
            {0x92, "FMUL.F"},
            {0x93, "FDIV.F"},
            {0x94, "FINV.F"},
            {0x95, "FSQRT.F"},
            {0xA0, "LD.K"},
            {0xA1, "ST.K"},
            {0xA2, "LD"},
            {0xA3, "ST"},
            {0xA4, "LDRGN"},
            {0xA5, "LDBU"},
            {0xA6, "LDBS"},
            {0xA7, "LDHU"},
            {0xA8, "LDHS"},
            {0xA9, "STB"},
            {0xAA, "STH"},
            {0xAB, "LDRIP"},
            {0xAC, "LL"},
            {0xAD, "SC"},
            {0xB0, "TXBEGIN"},
            {0xB1, "TXEND"},
            {0xB2, "TXABORT"},
            {0xC0, "SPAWN"},
            {0xC1, "YIELD"},
            {0xCB, "EXT"},
            {0xD0, "TLOG"},
            {0xD1, "PERF"},
            {0xD2, "PEEKPTE"},
            {0xD3, "RDBAD"},
            {0xD6, "CICPY"},
            {0xE0, "SYSRD"},
            {0xE1, "SYSWR"},
            {0xE2, "SYSCALL"},
            {0xE3, "WFI"},
            {0xE4, "IRET"},
            {0xE5, "ENI"},
            {0xE6, "DIS"},
            {0xE7, "FENCE"},
            {0xE8, "RDTSC"},
            {0xE9, "CPUID"},
            {0xEA, "FENCE.R"},
            {0xEB, "FENCE.W"},
            {0xEC, "FENCE.I"},
            {0xED, "BGE"},
            {0xEE, "BLT"},
            {0xEF, "BGT"},
            {0xF0, "BLE"},
            {0xF1, "BGEU"},
            {0xF2, "BLTU"},
            {0xF3, "BGTU"},
            {0xF4, "BLEU"},
            {0xF5, "PUSHR"},
            {0xF6, "POPR"},
            {0xF7, "PUSHI32"},
            {0xF8, "RRPIN"},
            {0xF9, "RRPIN.R"},
            {0xFA, "RRPIN.V"},
            {0xFB, "RRPIN.VR"},
            {0xFC, "SICF"},
            {0xFD, "OUTPRT.S"},
            {0xFE, "INPRT.S"},
        };
        static void ADC(Register rD, Register rS) => rD._value += rS + Register.STAT.GetBit(2);
        static void ADCI32(Register rD, uint imm32) => rD._value += imm32 + Register.STAT.GetBit(2);
        static void ADCI16(Register rD, ushort imm16) => rD._value += imm16 + Register.STAT.GetBit(2);
        static void ADCI8(Register rD, byte imm8) => rD._value += imm8 + Register.STAT.GetBit(2);

        static void ADD(Register rD, Register rS) => rD._value += rS;
        static void ADDI32(Register rD, uint imm32) => rD._value += imm32;
        static void ADDI16(Register rD, ushort imm16) => rD._value += imm16;
        static void ADDI8(Register rD, byte imm8) => rD._value += imm8;

        static void AND(Register rD, Register rS) => rD._value &= rS;
        static void ANDI32(Register rD, uint imm32) => rD._value &= imm32;
        static void ANDI16(Register rD, ushort imm16) => rD._value &= imm16;
        static void ANDI8(Register rD, byte imm8) => rD._value &= imm8;

        // Stolen from @AzureianGH's emulator
        static void LFLAG(uint value)
        {
            Register.STAT.SetBit(0, value == 0);
            Register.STAT.SetBit(1, (value & 0x80000000u) != 0);
            Register.STAT.SetBit(2, false);
            Register.STAT.SetBit(3, false);
            Register.STAT.SetBit(4, (BitOperations.PopCount((uint)value & 0xFF) & 1) == 0);
        }

        // Stolen from @AzureianGH's emulator
        static void ACMP(Register rA, Register rB,uint carry_in)
        {
            var result = rA + rB + carry_in;

            ulong wide = (ulong)rA + (ulong)rB + (ulong)carry_in;
            bool carry = (wide >> 32) != 0;
            bool overflow = (((rA ^ result) & (rB ^ result)) & 0x80000000u) != 0;

            Register.STAT.SetBit(0, result == 0);
            Register.STAT.SetBit(1, (result & 0x80000000u) != 0);
            Register.STAT.SetBit(2, carry);
            Register.STAT.SetBit(3, overflow);
            Register.STAT.SetBit(4, (BitOperations.PopCount((uint)result & 0xFF) & 1) == 0);
        }
        static void CMP(Register rA, Register rB)
        {
            var tmp = (int)rA - (int)rB;
            Register.STAT.SetBit(0,tmp == 0);
            Register.STAT.SetBit(1,tmp < 0);
            Register.STAT.SetBit(2,rA < rB);
            Register.STAT.SetBit(3, (((int)rA ^ (int)rB) & ((int)rA ^ tmp)) < 0);
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
        static void ORI16(Register rD, ushort imm16) => rD._value |= imm16;
        static void ORI8(Register rD, byte imm8) => rD._value |= imm8;

        static void XORI32(Register rD, uint imm32) => rD._value ^= imm32;
        static void XORI16(Register rD, ushort imm16) => rD._value ^= imm16;
        static void XORI8(Register rD, byte imm8) => rD._value ^= imm8;

        static void SUBI32(Register rD, uint imm32) => rD._value -= imm32;
        static void SUBI16(Register rD, ushort imm16) => rD._value -= imm16;
        static void SUBI8(Register rD, byte imm8) => rD._value -= imm8;

        static void ADCF(Register rD, Register rS) { ADC(rD, rS); ACMP(rD, rS, Register.STAT.GetBit(2)); }
        static void ADCI32F(Register rD, uint imm32) { ADCI32(rD, imm32); ACMP(rD, imm32, Register.STAT.GetBit(2)); }
        static void ADCI16F(Register rD, ushort imm16) { ADCI16(rD, imm16); ACMP(rD, imm16, Register.STAT.GetBit(2)); }
        static void ADCI8F(Register rD, byte imm8) { ADCI8(rD, imm8); ACMP(rD, imm8, Register.STAT.GetBit(2)); }

        static void ADDF(Register rD, Register rS) { ADD(rD, rS); ACMP(rD, rS,0); }
        static void ADDI32F(Register rD, uint imm32) { ADDI32(rD, imm32); ACMP(rD, imm32,0); }
        static void ADDI16F(Register rD, ushort imm16) { ADDI16(rD, imm16); ACMP(rD, imm16,0); }
        static void ADDI8F(Register rD, byte imm8) { ADDI8(rD, imm8); ACMP(rD, imm8,0); }

        static void ANDF(Register rD, Register rS) { AND(rD, rS); LFLAG(rD._value); }
        static void ANDI32F(Register rD, uint imm32) { ANDI32(rD, imm32); LFLAG(rD._value); }
        static void ANDI16F(Register rD, ushort imm16) { ANDI16(rD, imm16); LFLAG(rD._value); }
        static void ANDI8F(Register rD, byte imm8) { ANDI8(rD, imm8); LFLAG(rD._value); }

        static void SUBF(Register rD, Register rS) { SUB(rD, rS); CMP(rD, rS); }
        static void SUBI32F(Register rD, uint imm32) { SUBI32(rD, imm32); CMP(rD, imm32); }
        static void SUBI16F(Register rD, ushort imm16) { SUBI16(rD, imm16); CMP(rD, imm16); }
        static void SUBI8F(Register rD, byte imm8) { SUBI8(rD, imm8); CMP(rD, imm8); }

        static void ORF(Register rD, Register rS) { OR(rD, rS); LFLAG(rD._value); }
        static void XORF(Register rD, Register rS) { XOR(rD, rS); LFLAG(rD._value); }

        static void JR(Register abs32) => Register.IP = abs32._value;
        static void JA(uint abs32) => Register.IP = abs32;
        static void J(sbyte rel8) => Register.IP += rel8;
        static void J32(int rel32)
        {
            //Console.WriteLine($"{(Register.IP + rel32):X8} " + rel32.ToString("X8") + " " + Register.IP._value.ToString("X8"));
            Register.IP += rel32;
        }

        static void BZ(sbyte rel8) => Register.IP += Register.STAT.GetBit(0) == 1 ? rel8 : 0;
        static void BNZ(sbyte rel8) => Register.IP += Register.STAT.GetBit(0) == 0 ? rel8 : 0;

        static void BS(sbyte rel8) => Register.IP += Register.STAT.GetBit(1) == 1 ? rel8 : 0;
        static void BNS(sbyte rel8) => Register.IP += Register.STAT.GetBit(1) == 0 ? rel8 : 0;

        static void BO(sbyte rel8) => Register.IP += Register.STAT.GetBit(3) == 1 ? rel8 : 0;
        static void BNO(sbyte rel8) => Register.IP += Register.STAT.GetBit(3) == 0 ? rel8 : 0;

        static void RET()
        {
            var pip = Register.IP;
            Register.IP = Program.Memory.PopStack32();

            //Console.WriteLine(pip._value.ToString("X8") + " " + Register.IP._value.ToString("X8"));
        }

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
        static void BLEU(sbyte rel8) => Register.IP += Register.STAT.GetBit(2) == 1 || Register.STAT.GetBit(0) == 1 ? rel8 : 0;

        static void NOP() { }
        static void BRK() 
        { 
            Register.CTLregs[7] = (uint)EmulationFaultType.Breakpoint;
            Program.IsPaused = true; 
            Register.DumpRegister(); 
        }

        static void SPAWN(Register rD, Register rFn, Register rArg) => rD._value = Register.SpawnFiber(rFn,rArg);
        static void YIELD(Register rQ) => Register.SwitchFiber(rQ);

        static void LDK(Register rD, Register rA, sbyte off8)
        {
            uint addr = (uint)(rA._value + off8);
            if (Program.Memory.CheckCLT6(addr))
                rD._value = Program.Memory.ReadU32(addr);
            else throw new EmulationException(EmulationFaultType.AlignmentFault,addr,$"Unaligned memory access at address 0x{addr:X8}");

        }
        static void STK(Register rS,Register rA, sbyte off8)
        {
            uint addr = (uint)(rA._value + off8);

            if (Program.Memory.CheckCLT6(addr))
                Program.Memory.WriteU32(addr, rS._value);
            else throw new EmulationException(EmulationFaultType.AlignmentFault, addr,$"Unaligned memory access at address 0x{addr:X8}");
        }

        static void LD(Register rD, Register rA, sbyte off8) => rD._value = Program.Memory.ReadU32((uint)(rA._value + off8));
        static void ST(Register rS, Register rA, sbyte off8) => Program.Memory.WriteU32((uint)(rA._value + off8), rS._value);

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

        static void STB(Register rS, Register rA, sbyte off8) => Program.Memory[(uint)(rA._value + off8)] = (byte)rS._value;
        static void STH(Register rS,Register rA, sbyte off8) => Program.Memory.WriteU16((uint)(rA._value + off8), (ushort)rS._value);

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

        static void OUTPRTS(ushort port16,BytesSize2 size2, Register rD)
        {
            rD._value = Register.Bregs[0]._value;

            byte[] data = size2 switch
            {
                BytesSize2.Byte => new byte[] { (byte)rD._value },
                BytesSize2.HalfWord => BitConverter.GetBytes((ushort)rD._value),
                BytesSize2.Word => BitConverter.GetBytes(rD._value),
                _ => throw new EmulationException(EmulationFaultType.AlignmentFault, (uint)size2, $"Invalid byte size {size2}")
            };

            Debug.Write(Encoding.Default.GetString(data));
        }
        static void INPRTS(ushort port16, BytesSize2 size2, Register rD) => NOP();

        static void PUSHR(Register rS) => Program.Memory.PushStack(rS);
        static void PUSHI32(uint imm32) => Program.Memory.PushStack(imm32);
        static void POPR(Register rD) => rD._value = Program.Memory.PopStack32();

        static void RDSTK(Register rD) => rD._value = Register.STK;
        static void WRSTK(Register rS) => Register.STK = rS;

        static void RDFR(Register rD) => rD._value = Register.FR;
        static void WRFR(Register rS) => Register.FR = rS;

        static void ENTERI16(ushort frame16)
        {
            PFR();
            Register.FR = Register.STK;
            Register.STK._value -= frame16;
        }

        static void LEAVE() 
        {
            Register.STK = Register.FR;
            Register.FR = Program.Memory.PopStack32();
        }

        static void PSTAT() => Program.Memory.PushStack(Register.STAT._value);
        static void POPSTAT() => Register.STAT = Program.Memory.PopStack32();
        static void PCTL(CTLIdx3 n) => Program.Memory.PushStack(Register.CTLregs[(int)n]);
        static void POPCTL(CTLIdx3 n) => Register.CTLregs[(int)n] = Program.Memory.PopStack32();
        static void PFR() => Program.Memory.PushStack(Register.FR._value);
        static void POPFR() => Register.FR = Program.Memory.PopStack32();

        static void EXT(Register rD, Register rS, byte subop8) => NOP();
        static void SYSCALL()
        {
            uint callnmbr = Register.CTLregs[4]._value & 0xFF;

            Program.Memory.PushStack(Register.STAT._value);
            Program.Memory.PushStack(Register.IP._value);

            Register.CTLregs[0] = 0;

            uint vectorAddr = Register.CTLregs[1]._value + (4u * callnmbr);
            Register.IP._value = Program.Memory.ReadU32(vectorAddr);
        }
        static void WFI()
        {
            if (Register.CTLregs[0] == 1)
                throw new EmulationException(EmulationFaultType.ProtectionFault, 0, $"Tried to wait for interrupt in user mode");

            while (!Program.InterruptPending) Thread.Sleep(1);
        }
        static void IRET()
        {
            if (Register.CTLregs[0] == 1)
                throw new EmulationException(EmulationFaultType.ProtectionFault, 0, $"Tried to return from interrupt in user mode");

            Register.IP._value = Program.Memory.PopStack32();
            Register.STAT._value = Program.Memory.PopStack32();

            Register.CTLregs[0] = 1;
        }
        static void ENI()
        {
            if (Register.CTLregs[0] == 1)
                throw new EmulationException(EmulationFaultType.ProtectionFault, 0, $"Tried to enable interrupts in user mode");

            Register.STAT.SetBit(5,true);
        }
        static void DIS()
        {
            if (Register.CTLregs[0] == 1)
                throw new EmulationException(EmulationFaultType.ProtectionFault, 0, $"Tried to disable interrupts in user mode");

            Register.STAT.SetBit(5, false);
        }
        static void SYSRD(Register rD, CTLIdx3 ctlidx)
        {
            if (Register.CTLregs[0] == 1 && ((byte)ctlidx != 2 && (byte)ctlidx != 5))
                throw new EmulationException(EmulationFaultType.ProtectionFault,(uint)ctlidx,$"Tried to access CTL{ctlidx} in user mode");

            rD._value = Register.CTLregs[(int)ctlidx];
        }
        static void SYSWR(Register rS, CTLIdx3 ctlidx)
        {
            if (Register.CTLregs[0] == 1 && (byte)ctlidx != 5)
                throw new EmulationException(EmulationFaultType.ProtectionFault, (uint)ctlidx,$"Tried to write CTL{ctlidx} in user mode");

            Register.CTLregs[(int)ctlidx] = rS._value;
        }
        static void RDBAD(Register rD) => rD._value = Register.BADADDR;
        static void RDTSC(Register rD) => rD._value = Program.CycleCount;
        static void CPUID(byte sel8)
        {
            Register.CTLregs[3]._value = 0;
            switch (sel8)
            {
                case 1:
                    Register.Bregs[0]._value = 0x68536C42;
                    Register.Bregs[1]._value = 0x20323320;
                    Register.Bregs[2]._value = 0x55504320;
                    Register.Bregs[3]._value = 0x36323032;
                    break;
                case 2:
                    var now = DateTime.UtcNow;
                    Register.Bregs[0]._value = (uint)(now.Year * 10000 + now.Month * 100 + now.Day);
                    break;
                case 3:
                    Register.Bregs[0]._value = 0x00000000;
                    Register.Bregs[1]._value = 0x00000001;
                    break;
                case 4:
                    Register.Bregs[0]._value = 0x00000001;
                    Register.Bregs[1]._value = 0;
                    Register.Bregs[2]._value = 0;
                    Register.Bregs[3]._value = 0;
                    break;
                default:
                    break;
            }
        }

        static void SICF(byte imm8)
        {
            Register.CTLregs[7] = imm8;
            Program.Memory.PushStack(Register.IP._value);

            Register.IP = Program.Memory.ReadU32(Register.CTLregs[1] + (4 * imm8));
        }
    }
}
