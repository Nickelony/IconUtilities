using System.Drawing.Imaging;
using System.Drawing.Interfaces;
using System.Drawing.Objects;
using System.Drawing.Structs;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	public static class IconUtilities
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

		/// <summary>
		/// Injects .ico files into .exe files, therefore replacing their icon.
		/// </summary>
		public static void InjectIcon(string exeFilePath, string iconFilePath)
			=> InjectIcon(exeFilePath, iconFilePath, 1, 1);

		/// <summary>
		/// Injects .ico files into .exe files, therefore replacing their icon.
		/// </summary>
		public static void InjectIcon(string exeFilePath, string iconFilePath, uint iconGroupID, uint iconBaseID)
		{
			var iconFile = IconFile.FromFile(iconFilePath);

			IntPtr hUpdate = NativeMethods.BeginUpdateResource(exeFilePath, false);
			byte[] data = iconFile.CreateIconGroupData(iconBaseID);

			NativeMethods.UpdateResource(hUpdate, new IntPtr(NativeMethods.RT_GROUP_ICON), new IntPtr(iconGroupID), 0, data, data.Length);

			for (int i = 0; i < iconFile.IconCount; i++)
			{
				byte[] image = iconFile.ImageData(i);
				NativeMethods.UpdateResource(hUpdate, new IntPtr(NativeMethods.RT_ICON), new IntPtr(iconBaseID + i), 0, image, image.Length);
			}

			NativeMethods.EndUpdateResource(hUpdate, false);

			// Update icon cache
			NativeMethods.SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
		}
	}
}
