using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileSizeVisualizer2
{
	/// <summary>
	/// Interaction logic for FileViewer.xaml
	/// </summary>
	public partial class FileViewer : UserControl
	{
		public BrowserFile File;
		public delegate void Navigator(string path);

		private readonly Navigator Navigate;

		public FileViewer(BrowserFile file, Navigator navigate)
		{
			InitializeComponent();
			this.File = file;
			Icon icon = DefaultIcons.FolderLarge;
			Navigate = navigate;

			if (file.FileType == BrowserFile.FileTypes.File 
				&& System.Drawing.Icon.ExtractAssociatedIcon(file.Path) is Icon _icon)
			{
				icon = _icon;
			}

			MemoryStream strm = new();
			icon.Save(strm);
			IconBitmapDecoder ibd = new(strm, BitmapCreateOptions.None, BitmapCacheOption.None);
			iconImage.Source = ibd.Frames[0];
			filePath.Text = file.Path;
			fileSize.Text = Formatting.FormatFileSize(file.Size);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (File.FileType == BrowserFile.FileTypes.Folder)
			{
				Navigate(File.Path);
			}
			else
			{
				Process.Start("explorer.exe", File.Path);
			}
		}
	}
}
