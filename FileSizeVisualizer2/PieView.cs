using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FileSizeVisualizer2
{
	public class PieView : FrameworkElement
	{
		internal List<FileViewer> viewers = new();
		internal long maxSize = 0;
		readonly double radius = 75;

		public PieView(List<FileViewer> views, long _maxSize)
		{
			viewers = views;
			this.maxSize = _maxSize;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			//double angle = 0.0;
			//Color start = Colors.Red;
			//Color end = Colors.Green;

			//long maxFile = 0;
			//for (int i = 0; i < Math.Min(viewers.Count, 6); i++)
			//{
			//	if (viewers[i].File.Size > maxFile)
			//		maxFile = viewers[i].File.Size;
			//}
			//drawingContext.DrawEllipse(Brushes.LightGray, null, new Point(10 + radius,radius), radius, radius);
			//for (int i = 0; i < Math.Min(viewers.Count, 6); i++)
			//{
			//	double proportion = ((double)viewers[i].File.Size) / maxSize;
			//	double propAngle = 2 * Math.PI * proportion;
			//	double degAngle = propAngle * 180 / Math.PI;
			//	//Color color = LerpColor(start, end, (float)viewers[i].File.Size / maxFile * 1 / (i + 1));
			//	Color color = LerpColor(start, end, i / (float)(Math.Min(viewers.Count, 6)-1));
			//	Brush b = new SolidColorBrush(color);
			//	Pen p = new(Brushes.Gray, 1);
			//	Rect r = new(10, 0, 2 * radius, 2 * radius);

			//	drawingContext.DrawArc(p, b, r, angle, degAngle);
			//	angle += degAngle;
			//}

			//long totalSize = viewers.Sum(s => s.File.Size);
			////drawingContext.DrawRectangle(Brushes.Red, null, new Rect(0, 0, 100, 100));
			////base.OnRender(drawingContext);
		}

		private static float[] ToHSV(Color color)
		{
			System.Drawing.Color sdCol = System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));

			float hue = sdCol.GetHue();
			float saturation = (max == 0) ? 0 : 1f - (1f * min / max);
			float value = max / 255f;
			return new float[] { hue, saturation, value };
		}
		private static Color HsvToRgb(float hue, float saturation, float value)
		{
			int hi = ((int)Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);

			value *= 255;
			byte v =(byte)value;
			byte p = (byte)(value * (1 - saturation));
			byte q = (byte)(value * (1 - f * saturation));
			byte t = (byte)(value * (1 - (1 - f) * saturation));

			if (hi == 0)
				return Color.FromArgb(255, v, t, p);
			else if (hi == 1)
				return Color.FromArgb(255, q, v, p);
			else if (hi == 2)
				return Color.FromArgb(255, p, v, t);
			else if (hi == 3)
				return Color.FromArgb(255, p, q, v);
			else if (hi == 4)
				return Color.FromArgb(255, t, p, v);
			else
				return Color.FromArgb(255, v, p, q);
		}
		private static Color LerpColor(Color startColor, Color endColor, float weight)
		{
			float[] sHsv = ToHSV(startColor);
			float[] eHsv = ToHSV(endColor);
			float[] oHsv = new float[3];

			oHsv[0] = sHsv[0] * (1 - weight) + eHsv[0] * weight;
			oHsv[1] = sHsv[1] * (1 - weight) + eHsv[1] * weight;
			oHsv[2] = sHsv[2] * (1 - weight) + eHsv[2] * weight;


			return HsvToRgb(oHsv[0], oHsv[1], oHsv[2]);
		}
	}
}
