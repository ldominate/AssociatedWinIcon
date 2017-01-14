using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace AssociatedWinIcon
{
	public class AssociatedWinIcon : IAssociatedWinIcon
	{
		private readonly IDictionary<string, IconInfo> _iconInfos;

		// WinAPI function
		[DllImport("shell32.dll", EntryPoint = "ExtractIconA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		private static extern IntPtr ExtractIcon(int hInst, string lpszExeFileName, int nIconIndex);

		public AssociatedWinIcon()
		{
			using (var extRoot = Registry.ClassesRoot)
			{
				if (extRoot == null) return;

				_iconInfos = new Dictionary<string, IconInfo>();

				foreach (var key in extRoot.GetSubKeyNames())
				{
					// skip if it is non file extension  key
					if (string.IsNullOrEmpty(key) || (key.IndexOf(".", StringComparison.Ordinal) != 0)) continue;

					var extKey = extRoot.OpenSubKey(key);

					// skip if no such element
					if ((extKey == null) || (extKey.GetValue("") == null)) continue;

					// get sub key
					var iconKey = string.Format("{0}\\DefaultIcon", extKey.GetValue(""));

					using (var extIcon = extRoot.OpenSubKey(iconKey))
					{
						// skip if no such element
						if ((extIcon == null) || (extIcon.GetValue("") == null)) continue;

						var iconPathIndex = extIcon.GetValue("").ToString();

						var iconInfo = new IconInfo { Extension = key };

						var strIndex = iconPathIndex.IndexOf(",", StringComparison.Ordinal);

						iconInfo.IconPath = strIndex > 0 ? iconPathIndex.Substring(0, strIndex) : iconPathIndex;

						int iconIndex;

						int.TryParse(iconPathIndex.Substring(strIndex + 1), out iconIndex);

						iconInfo.IconIndex = iconIndex;

						_iconInfos.Add(key, iconInfo);
					}
				}
			}
		}

		public Icon GetIcon(string extension)
		{
			if (extension.IndexOf(".", StringComparison.Ordinal) < 0) extension = string.Format(".{0}", extension);

			if (!_iconInfos.ContainsKey(extension)) return null;

			var iconInfo = _iconInfos[extension];

			if (iconInfo.Icon != null) return iconInfo.Icon;

			return iconInfo.Icon = ExtractIcon(iconInfo);
		}

		private static Icon ExtractIcon(IconInfo iconInfo)
		{
			var hIcon = ExtractIcon(0, iconInfo.IconPath, iconInfo.IconIndex);

			return hIcon != IntPtr.Zero ? Icon.FromHandle(hIcon) : null;
		}

	}
}