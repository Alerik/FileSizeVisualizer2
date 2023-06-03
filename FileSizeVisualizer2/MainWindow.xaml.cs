using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.Serialization;
using System.Drawing;

namespace FileSizeVisualizer2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private readonly DispatcherTimer loadTimer;
		private bool indexLoaded = false;
		public bool IndexLoaded { get { return indexLoaded; } private set { indexLoaded = value; OnPropertyChanged(); } }

		private int fileCount = 0;
		public int FileCount { get { return fileCount; } private set { fileCount = value; OnPropertyChanged(); } }

		private int loadTime = 0;
		public int LoadTime {get { return loadTime; }  private set { loadTime = value; OnPropertyChanged(); } }


		private long totalSize = 0;
		public long TotalSize {get { return totalSize; }  private set { totalSize = value; OnPropertyChanged(); } }

		private string formattedSize = "";
		public string FormattedSize { get { return formattedSize; } private set { formattedSize = value; OnPropertyChanged(); } }
		public FontAwesomeIcon LoadingIcon { get; private set; }
		private readonly FileIndex index;
		private PieView? pieView;
		public static string RootDirectory
		{
			get => RootDirectory;
			set
			{
				
			}
		}

		//private static string rootDirectory = "C:\\Users\\jacks\\test";
		private readonly static string rootDirectory = @"C:\Users\Jackson\Professional";

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string? name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private void OnFilesLoaded(object sender, FileLoader.FilesLoadedArgs args)
		{
			FileCount = args.Count;
		}

		private void OnFileSize(object sender, FileLoader.FileSizeArgs args)
		{
			TotalSize = args.Size;
			FormattedSize = Formatting.FormatFileSize(TotalSize);
		}

		public MainWindow()
		{
			InitializeComponent();
			loadTimer = new DispatcherTimer();
			loadTimer.Tick += (o, e) => LoadTime++;
			loadTimer.Interval = new TimeSpan(0, 0, 1);
			index = new FileIndex(rootDirectory);

			Array iconValues = Enum.GetValues(typeof(FontAwesomeIcon));

			if (iconValues.GetValue(new Random().Next(iconValues.Length)) is object icon)
				LoadingIcon = (FontAwesomeIcon)icon;
			else
				LoadingIcon = FontAwesomeIcon.Cog;

			FileLoader.OnFilesLoaded += OnFilesLoaded;
			FileLoader.OnFileSize += OnFileSize;
			DataContext = this;
		}

		private async void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			DispatcherTimer loadTimer = new();
			loadTimer.Tick += (o, e) => LoadTime++;
			loadTimer.Interval = new TimeSpan(0, 0, 1);
			loadTimer.Start();
			await FileLoader.Load(index.Root);
			IndexLoaded = true;
			loadTimer.Stop();
			
			LoadDirectory();
		}

		private void LoadDirectory()
		{
			filePanel.Children.Clear();

			List<FileViewer> viewers = new();

			foreach (BrowserFile folder in index.Top.Folders)
			{
				viewers.Add(new FileViewer(folder, Navigate));
			}
			foreach (BrowserFile file in index.Top.Files)
			{
				viewers.Add(new FileViewer(file, Navigate));
			}
			viewers.Sort((a, b) => b.File.Size.CompareTo(a.File.Size));

			foreach (FileViewer view in viewers)
			{
				filePanel.Children.Add(view);
			}

			List<FileViewer> large = new();

			for (int i = 0; i < 5; i++)
			{
				if (i >= viewers.Count)
					break;
				large.Add(viewers[i]);
			}

			if(pieView != null)
			{

			}
			pieView = new PieView(large, index.Top.Size);
			grid.Children.Add(pieView);
			pieView.SetValue(Grid.ColumnProperty, 1);
			pieView.SetValue(Grid.RowProperty, 1);
			pieView.InvalidateVisual();
		}

		public void Navigate(string path)
		{
			try
			{
				int navIndex = index.Top.Children?.FindIndex(f => f.Path == path) ?? 0;
				index.Navigate(navIndex);
				LoadDirectory();
			}
			catch (InvalidOperationException e)
			{
				Console.WriteLine(e);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			index.Back();
			LoadDirectory();
		}
	}
}
