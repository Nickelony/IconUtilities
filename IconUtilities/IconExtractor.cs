using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace IconUtilities
{
	public static class IconExtractor
	{
		/// <summary>
		/// Extracts an icon from any file type.
		/// </summary>
		public static Icon ExtractIcon(string filePath, IconSize size)
		{
			Icon icon = null;
			var fileInfo = new SHFILEINFOW();

			if (NativeMethods.SHGetFileInfoW(filePath, NativeMethods.FILE_ATTRIBUTE_NORMAL, ref fileInfo, Marshal.SizeOf(fileInfo), NativeMethods.SHGFI_SYSICONINDEX) == 0)
				throw new FileNotFoundException();

			var iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
			IImageList imageList = null;

			NativeMethods.SHGetImageList((int)size, ref iidImageList, ref imageList);

			if (imageList != null)
			{
				var hIcon = IntPtr.Zero;
				imageList.GetIcon(fileInfo.iIcon, (int)NativeMethods.ILD_IMAGE, ref hIcon);
				icon = Icon.FromHandle(hIcon).Clone() as Icon;

				NativeMethods.DestroyIcon(hIcon);

				if (icon.ToBitmap().PixelFormat != PixelFormat.Format32bppArgb)
				{
					icon.Dispose();

					imageList.GetIcon(fileInfo.iIcon, (int)NativeMethods.ILD_TRANSPARENT, ref hIcon);
					icon = Icon.FromHandle(hIcon).Clone() as Icon;

					NativeMethods.DestroyIcon(hIcon);
				}
			}

			return icon;
		}
	}
}
