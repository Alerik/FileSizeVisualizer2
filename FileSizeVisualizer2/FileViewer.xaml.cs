using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
		public static readonly DependencyProperty FileSizeProperty = DependencyProperty.Register("FileSize", typeof(string), typeof(FileViewer), new FrameworkPropertyMetadata(""));
		public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(FileViewer), new FrameworkPropertyMetadata(""));
		public static readonly DependencyProperty FileIconProperty = DependencyProperty.Register("FileIcon", typeof(Icon), typeof(FileViewer), new FrameworkPropertyMetadata(null));

		public FileViewer()
		{
			InitializeComponent();
		}
	}
}
