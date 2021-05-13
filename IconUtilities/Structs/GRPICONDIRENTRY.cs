using System.Runtime.InteropServices;

namespace System.Drawing.Structs
{
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct GRPICONDIRENTRY
	{
		public byte Width;
		public byte Height;
		public byte ColorCount;
		public byte Reserved;
		public ushort Planes;
		public ushort BitCount;
		public int BytesInRes;
		public ushort ID;
	}
}
