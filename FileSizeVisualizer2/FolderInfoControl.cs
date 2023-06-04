using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileSizeVisualizer2
{
	public class FolderInfoControl : FrameworkElement
	{
		public static readonly DependencyProperty FilesProperty = DependencyProperty.Register(nameof(Files), typeof(List<BrowserFile>), typeof(FolderInfoControl), new FrameworkPropertyMetadata(new List<BrowserFile>(), FrameworkPropertyMetadataOptions.AffectsRender));

		public List<BrowserFile> Files
		{
			get { return (List<BrowserFile>)GetValue(FilesProperty); } set { SetValue(FilesProperty, value); }
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			double margin = 20;
			////base.OnRender(drawingContext);
			//Rect pieChart = new Rect(10, 10, Width, Width);

			//double lastAngle = 0.0;
			//for (int i = 0; i < Files.Count; i++)
			//{
			//	double arcAngle = 360 / Files.Count;
			//	drawingContext.DrawArc(new Pen(Brushes.Red, 3), Brushes.Blue, pieChart, lastAngle, lastAngle + arcAngle);
			//}			
			double angle = 0.0;

			Color end = Colors.Red;
			Color start = Colors.Green;
			double availableWidth = ActualWidth - 2 * margin;
			double radius = availableWidth/2;

			long totalFileSize = 0;
			for (int i = 0; i < Files.Count; i++)
			{
					totalFileSize += Files[i].Size;
			}
			drawingContext.DrawEllipse(Brushes.LightGray, null, new Point(margin + radius, margin + radius), radius, radius);
			for (int i = 0; i < Files.Count; i++)
			{
				double proportion = ((double)Files[i].Size) / totalFileSize;
				double propAngle = 2 * Math.PI * proportion;
				double degAngle = propAngle * 180 / Math.PI;
				Color color = PieView.LerpColor(start, end, Files[i].Size * 1.0f / totalFileSize);
				Brush b = new SolidColorBrush(color);
				Pen p = new(Brushes.Gray, 1);
				Point center = new Point(margin + radius, margin + radius);

				drawingContext.DrawArc(p, b, center, radius, angle, degAngle);
				angle += degAngle;
			}

			long totalSize = Files.Sum(s => s.Size);
			//drawingContext.DrawRectangle(Brushes.Red, null, new Rect(0, 0, 100, 100));
			//base.OnRender(drawingContext);

		}

		protected override Size MeasureOverride(Size constraint)
		{
			return base.MeasureOverride(constraint);
		}
	}
}
