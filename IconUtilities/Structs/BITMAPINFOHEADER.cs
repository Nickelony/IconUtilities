﻿using System.Runtime.InteropServices;

namespace System.Drawing.Structs
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct BITMAPINFOHEADER
	{
		public uint Size;
		public int Width;
		public int Height;
		public ushort Planes;
		public ushort BitCount;
		public uint Compression;
		public uint SizeImage;
		public int XPelsPerMeter;
		public int YPelsPerMeter;
		public uint ClrUsed;
		public uint ClrImportant;
	}
}
