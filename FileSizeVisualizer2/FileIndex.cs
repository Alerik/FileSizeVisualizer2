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

		public FileIndex(string rootPath)
		{
			Root = new BrowserFile(rootPath, BrowserFile.FileTypes.Folder);
			fileStack = new Stack<BrowserFile>();
			fileStack.Push(Root);
		}

		public void Navigate(int index)
		{
			if (Top.Children is List<BrowserFile> children)
			{
				if (index >= Top.Count || index < 0)
					throw new IndexOutOfRangeException();
				if (children[index].FileType == BrowserFile.FileTypes.File)
					throw new InvalidOperationException();
				fileStack.Push(children[index]);
			}
			else
			{
				throw new InvalidOperationException("A non-folder BrowserFile was on the stack!");
			}
		}
		public void Back()
		{
			if (fileStack.Count > 1)
				fileStack.Pop();
		}
	}
}
