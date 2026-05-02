using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oblique
{
    public class PagedArray
    {
        private const int PAGE_SIZE = 4096;
        private readonly Dictionary<uint, byte[]> _pages = new();
        private readonly Dictionary<uint, (uint size,uint key)> KeyedRegions = new();

        public byte this[uint addr]
        {
            get
            {
                var (page, offset) = Split(addr);
                return _pages.TryGetValue(page, out var data) ? data[offset] : (byte)0;
            }
            set
            {
                var (page, offset) = Split(addr);
                if (!_pages.ContainsKey(page))
                    _pages[page] = new byte[PAGE_SIZE];
                _pages[page][offset] = value;
            }
        }

        public void Clear() => _pages.Clear();

        public void Load(uint baseAddr, byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
                this[(uint)(baseAddr + i)] = data[i];
        }

        public uint ReadU32(uint addr) => 
            (uint)(this[addr] | this[addr + 1] << 8 | this[addr + 2] << 16 | this[addr + 3] << 24);

        public ushort ReadU16(uint addr) => (ushort)(this[addr] | this[addr + 1] << 8);

        public void PushStack(uint val)
        {
            Register.STK -= 4;
            WriteU32(Register.STK, val);
        }
        public uint PopStack()
        {
            var val = ReadU32(Register.STK);
            Register.STK += 4;
            return val;
        }

        public void WriteU32(uint addr, uint val)
        {
            this[addr] = (byte)(val);
            this[addr + 1] = (byte)(val >> 8);
            this[addr + 2] = (byte)(val >> 16);
            this[addr + 3] = (byte)(val >> 24);
        }

        public void WriteU16(uint addr, ushort val)
        {
            this[addr] = (byte)(val);
            this[addr + 1] = (byte)(val >> 8);
        }

        public void ProtectRegion(uint baseAddr, uint size, uint key) => KeyedRegions[baseAddr] = (size, key);

        public bool IsRegionReadable(uint addr, uint length)
        {
            for (uint i = 0; i < length; i += PAGE_SIZE)
            {
                var page = (addr + i) / PAGE_SIZE;
                if (!_pages.ContainsKey(page))
                    return false;
            }
            return true;
        }

        public bool CheckCLT6(uint addr)
        {
            foreach (var (regionBase, (size, key)) in KeyedRegions)
                if (addr >= regionBase && addr < regionBase + size)
                    return Register.CTLregs[6] == key;

            return true;
        }

        private static (uint page, uint offset) Split(uint addr)
            => (addr / PAGE_SIZE, addr % PAGE_SIZE);

        public uint Length { get => (_pages.Keys.Max() + 1) * PAGE_SIZE - 1; }
    }
}
