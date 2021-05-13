using System.Runtime.InteropServices;

namespace IconUtilities
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ICONDIRENTRY
	{
		public byte Width;
		public byte Height;
		public byte ColorCount;
		public byte Reserved;
		public ushort Planes;
		public ushort BitCount;
		public int BytesInRes;
		public int ImageOffset;
	}
}
