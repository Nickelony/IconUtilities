using System.Runtime.InteropServices;

namespace System.Drawing.Structs
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ICONDIR
	{
		public ushort Reserved;
		public ushort Type;
		public ushort Count;
	}
}
