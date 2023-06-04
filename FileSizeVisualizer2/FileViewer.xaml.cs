using System.Drawing;
using System.Windows;
using System.Windows.Controls;

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
