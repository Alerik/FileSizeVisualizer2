using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSizeVisualizer2
{
	public class FileIndex
	{
		public readonly BrowserFile Root;
		private readonly Stack<BrowserFile> fileStack;
		public BrowserFile Top => fileStack.Peek();

		public bool Loaded => Top.Loaded;
		private const int threadCount = 20;

		private int loaded = 0;
		private int activeFiles = 0;

		public FileIndex()
		{

		}

		public FileIndex(string rootPath)
		{
			Root = new BrowserFile(rootPath, BrowserFile.FileTypes.Folder);
			fileStack = new Stack<BrowserFile>();
			fileStack.Push(Root);
		}

		public void Navigate(int index)
		{
			if (index >= Top.Count || index < 0)
				throw new IndexOutOfRangeException();
			if (Top.Children[index].FileType == BrowserFile.FileTypes.File)
				throw new InvalidOperationException();
			fileStack.Push(Top.Children[index]);
		}
		public void Back()
		{
			if (fileStack.Count > 1)
				fileStack.Pop();
		}

		//You probably need to re-think this or re-design it
		// Right now, it will load everything from the root directory, and if the root dir is < 200 files, it WILL NOT load everything

			}
}
