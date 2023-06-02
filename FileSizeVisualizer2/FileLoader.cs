using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSizeVisualizer2
{
	public static class FileLoader
	{
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

		public static async Task<bool> Load(BrowserFile file)
		{
			if (file.FileType == BrowserFile.FileTypes.File)
			{
				try
				{
					file.Size = await GetFileSizeAsync(file.Path);
				}
				catch (System.IO.FileNotFoundException e)
				{
					Console.WriteLine("Unable to find file " + file.Path);
					file.Size = -1;
					return false;
				}
				catch (System.Security.SecurityException e)
				{
					Console.WriteLine("Unable to access " + file.Path);
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
					files = new string[0];
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
					folders = new string[0];
					return false;
				}

				foreach (string childFolder in folders)
				{
					BrowserFile childBrowserFolder = new BrowserFile(childFolder, BrowserFile.FileTypes.Folder);
					await Load(childBrowserFolder);
					file.AddChild(childBrowserFolder);
				}

				foreach (string childFile in files)
				{
					BrowserFile childBrowserFile = new BrowserFile(childFile, BrowserFile.FileTypes.File);
					await Load(childBrowserFile);
					file.AddChild(childBrowserFile);
				}
			}
			file.Loaded = true;
			return true;
		}
	}
}
