using System.Collections.Generic;
using System.Linq;

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

		public BrowserFile(string path, FileTypes fileType)
		{
			this.Path = path;
			this.FileType = fileType;
			if (FileType == FileTypes.Folder)
			{
				Children = new List<BrowserFile>();
			}
		}

		public void AddChild(BrowserFile file)
		{
			Children?.Add(file);
			Size += file.Size;
		}
	}
}
