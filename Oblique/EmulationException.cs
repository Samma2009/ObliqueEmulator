using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oblique
{

	[Serializable]
	public class EmulationException : Exception
	{
		public EmulationException() { }
		public EmulationException(string message) : base(message) { }
		public EmulationException(string message, Exception inner) : base(message, inner) { }
		protected EmulationException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { Register.DumpRegister(); }
    }
}
