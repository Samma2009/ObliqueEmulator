using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oblique
{
    public class Fiber
    {
        public Register[] Bregs = new Register[16];
        public Register IP = new(), STK = new(), FR = new(), STAT = new();

        public Fiber(uint ip = 0xA0000000, uint stk = 0x20000000)
        {
            for (int i = 0; i < Bregs.Length; i++) Bregs[i] = new();

            IP = new(ip);
            STK = new(stk);
            FR = new();
            STAT = new();
        }
    }
}
