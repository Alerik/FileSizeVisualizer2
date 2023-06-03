using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace FileSizeVisualizer2
{
	public static class FileLoader
	{
		private static int loadedFiles = 0;
		public static int LoadedFiles
		{
			get
			{
				return loadedFiles;
			}
			set
			{
				loadedFiles = value;
				OnFilesLoaded?.Invoke(typeof(FileLoader), new FilesLoadedArgs(loadedFiles));
			}
		}

		private static long totalSize = 0;
		public static long TotalSize
		{
			get
			{
				return totalSize;
			}
			set
			{
				totalSize = value;
				OnFileSize?.Invoke(typeof(FileLoader), new FileSizeArgs(totalSize));
			}
		}
		public class FilesLoadedArgs : EventArgs
		{
			public readonly int Count;

			public FilesLoadedArgs(int count)
			{
				Count = count;	
			}
		}
		public class FileSizeArgs : EventArgs
		{
			public readonly long Size;

			public FileSizeArgs(long size)
			{
				Size = size;	
			}
		}

		public delegate void FilesLoadedHandler(object source, FilesLoadedArgs e);
		public static event FilesLoadedHandler? OnFilesLoaded;

		public delegate void FilesSizeHandler(object source, FileSizeArgs e);
		public static event FilesSizeHandler? OnFileSize;
		private static async Task<long> GetFileSizeAsync(string path)
		{
			return await Task.Run(() => new FileInfo(path).Length);
		}

		private static async Task<string[]> GetFilesAsync(string path)
		{
			return await Task.Run(() => Directory.GetFiles(path));
		}

		private static async Task<string[]> GetFoldersAsync(string path)
		{
			return await Task.Run(() => Directory.GetDirectories(path));
		}

		public static async Task<bool> StartLoad(BrowserFile file)
		{
			LoadedFiles = 0;
			TotalSize = 0;
			return await Load(file);
		}
		private static async Task<bool> Load(BrowserFile file)
		{
			if (file.FileType == BrowserFile.FileTypes.File)
			{
				try
				{
					file.Size = await GetFileSizeAsync(file.Path);
				}
				catch (System.IO.FileNotFoundException e)
				{
					Trace.WriteLine($"Unable to find file {file.Path}\n{e}");
					file.Size = -1;
					return false;
				}
				catch (System.Security.SecurityException e)
				{
					Trace.WriteLine($"Unable to access {file.Path} \n {e}");
					file.Size = -1;
					return false;
				}
				file.Loaded = true;
				return true;
			}
			else
			{
				string[] files;
				try
				{
					files = await GetFilesAsync(file.Path);
				}
				catch (System.UnauthorizedAccessException e)
				{
					Console.WriteLine(e);
					files = Array.Empty<string>();
					return false;
				}
				string[] folders;
				try
				{
					folders = await GetFoldersAsync(file.Path);
				}
				catch (System.UnauthorizedAccessException e)
				{
					Console.WriteLine(e);
					folders = Array.Empty<string>();
					return false;
				}

				foreach (string childFile in files)
				{
					BrowserFile childBrowserFile = new(childFile, BrowserFile.FileTypes.File);
					await Load(childBrowserFile);
					file.AddChild(childBrowserFile);
				}

				LoadedFiles += files.Length;

				long size = 0;
				foreach (string childFolder in folders)
				{
					BrowserFile childBrowserFolder = new(childFolder, BrowserFile.FileTypes.Folder);
					await Task.Run(() => Load(childBrowserFolder));
					file.AddChild(childBrowserFolder);
					size += childBrowserFolder.Size;
				}

				TotalSize += size;
			}
			file.Loaded = true;
			return true;
		}
	}
}
