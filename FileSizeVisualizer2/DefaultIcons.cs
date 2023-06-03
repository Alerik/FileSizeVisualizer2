using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileSizeVisualizer2
{
	public static class DefaultIcons
	{
		private static readonly Lazy<Icon> _lazyFolderIcon = new(FetchIcon, true);

		public static Icon FolderLarge
		{
			get { return _lazyFolderIcon.Value; }
		}

		private static Icon FetchIcon()
		{
			var tmpDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())).FullName;
			var icon = ExtractFromPath(tmpDir);
			Directory.Delete(tmpDir);
			return icon;
		}

		private static Icon ExtractFromPath(string path)
		{
			SHFILEINFO shinfo = new();
			SHGetFileInfo(
				path,
				0, ref shinfo, (uint)Marshal.SizeOf(shinfo),
				SHGFI_ICON | SHGFI_LARGEICON);
			return System.Drawing.Icon.FromHandle(shinfo.hIcon);
		}

		//Struct used by SHGetFileInfo function
		[StructLayout(LayoutKind.Sequential)]
		private struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		[DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

		private const uint SHGFI_ICON = 0x100;
		private const uint SHGFI_LARGEICON = 0x0;
		private const uint SHGFI_SMALLICON = 0x000000001;
	}
}
