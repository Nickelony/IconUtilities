using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IconUtilities
{
	internal class IconFile
	{
		#region Properties

		public int IconCount { get { return _iconDir.Count; } }

		#endregion Properties

		#region Fields

		private ICONDIR _iconDir = new ICONDIR();
		private ICONDIRENTRY[] _iconDirEntry;

		private byte[][] _iconImage;

		#endregion Fields

		#region Methods

		public byte[] ImageData(int index)
			=> _iconImage[index];

		public static IconFile FromFile(string filename)
		{
			var instance = new IconFile();

			byte[] fileBytes = File.ReadAllBytes(filename);
			GCHandle pinnedBytes = GCHandle.Alloc(fileBytes, GCHandleType.Pinned);

			instance._iconDir = (ICONDIR)Marshal.PtrToStructure(pinnedBytes.AddrOfPinnedObject(), typeof(ICONDIR));
			instance._iconDirEntry = new ICONDIRENTRY[instance.IconCount];
			instance._iconImage = new byte[instance.IconCount][];

			int offset = Marshal.SizeOf(instance._iconDir);
			int size = Marshal.SizeOf(typeof(ICONDIRENTRY));

			for (int i = 0; i < instance.IconCount; i++)
			{
				var entry = (ICONDIRENTRY)Marshal.PtrToStructure(new IntPtr(pinnedBytes.AddrOfPinnedObject().ToInt64() + offset), typeof(ICONDIRENTRY));
				instance._iconDirEntry[i] = entry;

				instance._iconImage[i] = new byte[entry.BytesInRes];

				Buffer.BlockCopy(fileBytes, entry.ImageOffset, instance._iconImage[i], 0, entry.BytesInRes);

				offset += size;
			}

			pinnedBytes.Free();
			return instance;
		}

		public byte[] CreateIconGroupData(uint iconBaseID)
		{
			int sizeOfIconGroupData = Marshal.SizeOf(typeof(ICONDIR)) + Marshal.SizeOf(typeof(GRPICONDIRENTRY)) * IconCount;
			byte[] data = new byte[sizeOfIconGroupData];

			GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
			Marshal.StructureToPtr(_iconDir, pinnedData.AddrOfPinnedObject(), false);

			int offset = Marshal.SizeOf(_iconDir);
			int size = Marshal.SizeOf(typeof(GRPICONDIRENTRY));

			for (int i = 0; i < IconCount; i++)
			{
				GRPICONDIRENTRY grpEntry = new GRPICONDIRENTRY();
				BITMAPINFOHEADER bitmapHeader = new BITMAPINFOHEADER();

				GCHandle pinnedBitmapInfoHeader = GCHandle.Alloc(bitmapHeader, GCHandleType.Pinned);
				Marshal.Copy(_iconImage[i], 0, pinnedBitmapInfoHeader.AddrOfPinnedObject(), Marshal.SizeOf(typeof(BITMAPINFOHEADER)));

				pinnedBitmapInfoHeader.Free();

				grpEntry.Width = _iconDirEntry[i].Width;
				grpEntry.Height = _iconDirEntry[i].Height;
				grpEntry.ColorCount = _iconDirEntry[i].ColorCount;
				grpEntry.Reserved = _iconDirEntry[i].Reserved;
				grpEntry.Planes = bitmapHeader.Planes;
				grpEntry.BitCount = bitmapHeader.BitCount;
				grpEntry.BytesInRes = _iconDirEntry[i].BytesInRes;
				grpEntry.ID = Convert.ToUInt16(iconBaseID + i);

				Marshal.StructureToPtr(grpEntry, new IntPtr(pinnedData.AddrOfPinnedObject().ToInt64() + offset), false);

				offset += size;
			}

			pinnedData.Free();
			return data;
		}

		#endregion Methods
	}
}
