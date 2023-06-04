using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FileSizeVisualizer2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private readonly DispatcherTimer loadTimer;
		private bool indexLoaded = false;

		public string WindowTitle
		{
			get
			{
				return $"File Size Visualizer ({RootDirectory})";
			}
		}

		public bool IndexLoaded { get { return indexLoaded; } private set { indexLoaded = value; OnPropertyChanged(); } }

		private int fileCount = 0;
		public int FileCount { get { return fileCount; } private set { fileCount = value; OnPropertyChanged(); } }

		private int loadTime = 0;
		public int LoadTime { get { return loadTime; } private set { loadTime = value; OnPropertyChanged(); } }


		private long totalSize = 0;
		public long TotalSize { get { return totalSize; } private set { totalSize = value; OnPropertyChanged(); } }

		private string formattedSize = "";
		public string FormattedSize { get { return formattedSize; } private set { formattedSize = value; OnPropertyChanged(); } }
		public FontAwesomeIcon LoadingIcon { get; private set; }
		private FileIndex? index;

		private const string START_DIRECTORY = "";
		private string rootDirectory;
		public string RootDirectory
		{
			get => rootDirectory;
			set
			{
				rootDirectory = value;
				OnPropertyChanged();
				OnPropertyChanged("WindowTitle");
			}
		}

		private string currentPath;
		public string CurrentPath
		{
			get => currentPath;
			set
			{
				currentPath = value;
				OnPropertyChanged();
			}
		}

		private List<BrowserFile> files = new();
		public List<BrowserFile> Files
		{
			get { return files; }
			private set { files = value; OnPropertyChanged(); }
		}

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

		public MainWindow() : this(START_DIRECTORY) { }
		public MainWindow(string startDirectory)
		{
			InitializeComponent();
			rootDirectory = startDirectory;
			currentPath = startDirectory;
			loadTimer = new DispatcherTimer();
			loadTimer.Tick += (o, e) => LoadTime++;
			loadTimer.Interval = new TimeSpan(0, 0, 1);

			Array iconValues = Enum.GetValues(typeof(FontAwesomeIcon));

			if (iconValues.GetValue(new Random().Next(iconValues.Length)) is object icon)
			{
				LoadingIcon = (FontAwesomeIcon)icon;
			}
			else
			{
				LoadingIcon = FontAwesomeIcon.Cog;
			}

			FileLoader.OnFilesLoaded += OnFilesLoaded;
			FileLoader.OnFileSize += OnFileSize;
			DataContext = this;
		}

		private void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(RootDirectory))
			{
				LoadIndex();
			}
		}

		private async void LoadIndex()
		{
			IndexLoaded = false;
			index = new FileIndex(RootDirectory);
			DispatcherTimer loadTimer = new();
			loadTimer.Tick += (o, e) => LoadTime++;
			loadTimer.Interval = new TimeSpan(0, 0, 1);
			loadTimer.Start();
			await FileLoader.StartLoad(index.Root);
			IndexLoaded = true;
			loadTimer.Stop();

			LoadDirectory();
		}

		private void LoadDirectory()
		{
			if (index is FileIndex _index)
			{
				List<FileViewer> viewers = new();

				List<BrowserFile> files = _index.Top.Folders.Concat(_index.Top.Files).ToList();
				files.Sort((a, b) => b.Size.CompareTo(a.Size));
				Files = files;
			}
		}

		public void Navigate(string path)
		{
			try
			{
				int navIndex = index?.Top.Children?.FindIndex(f => f.Path == path) ?? 0;
				index?.Navigate(navIndex);
				LoadDirectory();
			}
			catch (InvalidOperationException e)
			{
				Trace.WriteLine(e);
			}
			CurrentPath = path;
		}

		private void navBack_Click(object sender, RoutedEventArgs e)
		{

			index?.Back();
			LoadDirectory();
		}

		private void btnStartIndex_Click(object sender, RoutedEventArgs e)
		{
			RootDirectory = CurrentPath;
			LoadIndex();
		}

		private void txtNavbar_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				RootDirectory = txtNavbar.Text;
				LoadIndex();
			}
		}

		private void filePanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (filePanel.SelectedItem is BrowserFile file)
			{
				Navigate(file.FilePath);
			}
		}
	}
}
