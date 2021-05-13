using System;

namespace IconUtilities
{
	public static class IconInjector
	{
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
