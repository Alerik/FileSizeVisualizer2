using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace FileSizeVisualizer2
{
	public class BrowserFile
	{
		public enum FileTypes
		{
			File,
			Folder
		}

		public readonly string Path;
		public readonly FileTypes FileType;

		public List<BrowserFile>? Children { get; private set; }
		public string FilePath => Path;
		public string FormattedSize => Formatting.FormatFileSize(Size);
		public long Size { get; internal set; }
		public bool Loaded { get; internal set; }
		public int Count => Children?.Count ?? 0; 
		public List<BrowserFile> Folders => Children?.Select(f => f).Where(f => f.FileType == FileTypes.Folder).ToList() ?? new();
		public List<BrowserFile> Files => Children?.Select(f => f).Where(f => f.FileType == FileTypes.File).ToList() ?? new();

		public ImageSource FileIcon { get; set; }

		public BrowserFile(string path, FileTypes fileType)
		{
			this.Path = path;
			this.FileType = fileType;
			if (FileType == FileTypes.Folder)
				Children = new List<BrowserFile>();
		}

		public void AddChild(BrowserFile file)
		{
			Children?.Add(file);
			Size += file.Size;
		}

		public void LoadIcon()
		{
			Icon icon = DefaultIcons.FolderLarge;

			if (FileType == BrowserFile.FileTypes.File 
				&& System.Drawing.Icon.ExtractAssociatedIcon(FilePath) is Icon _icon)
			{
				icon = _icon;
			}

			MemoryStream strm = new();
			icon.Save(strm);
			IconBitmapDecoder ibd = new(strm, BitmapCreateOptions.None, BitmapCacheOption.None);
			FileIcon = ibd.Frames[0];
		}
	}
}
