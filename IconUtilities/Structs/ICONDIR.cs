using System.Runtime.InteropServices;

namespace IconUtilities
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ICONDIR
	{
		public ushort Reserved;
		public ushort Type;
		public ushort Count;
	}
}
