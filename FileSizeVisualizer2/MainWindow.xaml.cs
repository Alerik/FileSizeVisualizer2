﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileSizeVisualizer2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		FileIndex index;
		public static MainWindow Instance;
		private PieView pieView;
		public static string RootDirectory
		{
			get => RootDirectory;
			set
			{
				
			}
		}

		//private static string rootDirectory = "C:\\Users\\jacks\\test";
		private static string rootDirectory = @"C:\Users\Jackson\Professional";

		public MainWindow()
		{
			Instance = this;
			InitializeComponent();
			index = new FileIndex(rootDirectory);
		}

		private async void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			await FileLoader.Load(index.Root);
			LoadDirectory();
		}

		private void LoadDirectory()
		{
			filePanel.Children.Clear();

			List<FileViewer> viewers = new List<FileViewer>();

			foreach (BrowserFile folder in index.Top.Folders)
			{
				viewers.Add(new FileViewer(folder));
			}
			foreach (BrowserFile file in index.Top.Files)
			{
				viewers.Add(new FileViewer(file));
			}
			viewers.Sort((a, b) => b.File.Size.CompareTo(a.File.Size));

			foreach (FileViewer view in viewers)
			{
				filePanel.Children.Add(view);
			}

			List<FileViewer> large = new List<FileViewer>();

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
				int navIndex = index.Top.Children.FindIndex(f => f.Path == path);
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
