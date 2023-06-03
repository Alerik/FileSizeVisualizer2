﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileSizeVisualizer2
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		void App_Startup(object sender, StartupEventArgs e)
		{
			if (e.Args.Length > 0)
				MainWindow = new MainWindow(e.Args[0]);
			else
				MainWindow = new MainWindow();

			MainWindow.Show();
		}
	}
}
