using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FileSizeVisualizer2
{
	public static class Formatting
	{
		public static readonly long K = 1L << 10, M = 1L << 20, G = 1L << 30;

		//This is faster than doubles conversion and ceiling funcs
		private static long DivideUp(long m, long n)
		{
			return (m + n - 1) / n;
		}
		//private static double DivideUp(long m, long n)
		//{
		//	return m / (1.0*n);
		//}
		public static string FormatFileSize(long bytes)
		{
			if (bytes < K)
				return $"{bytes} bytes";
			if (bytes < M)
				return $"{DivideUp(bytes, K):#.##} Kb";
			if (bytes < G)
				return $"{bytes * 1.0 / M:#.##} Mb";
			return $"{bytes * 1.0 / G:#.##} Gb";
		}

		public static void DrawArc(this DrawingContext dc, Pen pen, Brush brush, Point center, double radius, double startDegrees, double sweepDegrees)
		{
			GeometryDrawing arc = CreateArcDrawing(center, radius, startDegrees, sweepDegrees);
			arc.Brush = brush;
			arc.Pen = pen;
			dc.DrawGeometry(brush, pen, arc.Geometry);
		}

		/// <summary>
		/// Create an Arc geometry drawing of an ellipse or circle
		/// </summary>
		/// <param name="rect">Box to hold the whole ellipse described by the arc</param>
		/// <param name="startDegrees">Start angle of the arc degrees within the ellipse. 0 degrees is a line to the right.</param>
		/// <param name="sweepDegrees">Sweep angle, -ve = Counterclockwise, +ve = Clockwise</param>
		/// <returns>GeometryDrawing object</returns>
		private static GeometryDrawing CreateArcDrawing(Point center, double radius, double startDegrees, double sweepDegrees)
		{
			// degrees to radians conversion
			double startRadians = startDegrees * Math.PI / 180.0;
			double sweepRadians = sweepDegrees * Math.PI / 180.0;

			// determine the start point 
			double xs = center.X + (Math.Cos(startRadians) * radius);
			double ys = center.Y + (Math.Sin(startRadians) * radius);

			// determine the end point 
			double xe = center.X + (Math.Cos(startRadians + sweepRadians) * radius);
			double ye = center.Y + (Math.Sin(startRadians + sweepRadians) * radius);

			// draw the arc into a stream geometry
			StreamGeometry streamGeom = new();
			using (StreamGeometryContext ctx = streamGeom.Open())
			{
				bool isLargeArc = Math.Abs(sweepDegrees) > 180;
				SweepDirection sweepDirection = sweepDegrees < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

				ctx.BeginFigure(new Point(xs, ys), true, true);
				ctx.ArcTo(new Point(xe, ye), new Size(radius, radius), 0, isLargeArc, sweepDirection, true, true);
				ctx.LineTo(new Point(center.X, center.Y), true, true);
				ctx.LineTo(new Point(xs, ys), true, true);
			}

			// create the drawing
			GeometryDrawing drawing = new()
			{
				Geometry = streamGeom
			};
			return drawing;
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
		public static Color HsvToRgb(float hue, float saturation, float value)
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
		public static Color LerpColor(Color startColor, Color endColor, float weight)
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
