using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oblique
{
    public interface IISAProvider 
    {
        public Dictionary<byte,Delegate> InstructionMap { get; set; }
        public Dictionary<byte,ushort> InstructionSizeMap { get; set; }
    }
}
