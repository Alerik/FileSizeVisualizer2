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
				return $"{(DivideUp(bytes, K)).ToString("#.##")} Kb";
			if (bytes < G)
				return $"{(bytes * 1.0 / M).ToString("#.##")} Mb";
			return $"{(bytes * 1.0 / G).ToString("#.##")} Gb";
		}

		public static void DrawArc(this DrawingContext dc, Pen pen, Brush brush, Rect rect, double startDegrees, double sweepDegrees)
		{
			GeometryDrawing arc = CreateArcDrawing(rect, startDegrees, sweepDegrees);
			arc.Brush = brush;
			arc.Pen = pen;
			bool s = arc.IsSealed;
			dc.DrawGeometry(brush, pen, arc.Geometry);
		//	arc.
		}

		/// <summary>
		/// Create an Arc geometry drawing of an ellipse or circle
		/// </summary>
		/// <param name="rect">Box to hold the whole ellipse described by the arc</param>
		/// <param name="startDegrees">Start angle of the arc degrees within the ellipse. 0 degrees is a line to the right.</param>
		/// <param name="sweepDegrees">Sweep angle, -ve = Counterclockwise, +ve = Clockwise</param>
		/// <returns>GeometryDrawing object</returns>
		private static GeometryDrawing CreateArcDrawing(Rect rect, double startDegrees, double sweepDegrees)
		{
			// degrees to radians conversion
			double startRadians = startDegrees * Math.PI / 180.0;
			double sweepRadians = sweepDegrees * Math.PI / 180.0;

			// x and y radius
			double dx = rect.Width / 2;
			double dy = rect.Height / 2;

			// determine the start point 
			double xs = rect.X + dx + (Math.Cos(startRadians) * dx);
			double ys = rect.Y + dy + (Math.Sin(startRadians) * dy);

			// determine the end point 
			double xe = rect.X + dx + (Math.Cos(startRadians + sweepRadians) * dx);
			double ye = rect.Y + dy + (Math.Sin(startRadians + sweepRadians) * dy);

			// draw the arc into a stream geometry
			StreamGeometry streamGeom = new StreamGeometry();
			using (StreamGeometryContext ctx = streamGeom.Open())
			{
				bool isLargeArc = Math.Abs(sweepDegrees) > 180;
				SweepDirection sweepDirection = sweepDegrees < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

				ctx.BeginFigure(new Point(xs, ys), true, true);
				ctx.ArcTo(new Point(xe, ye), new Size(dx, dy), 0, isLargeArc, sweepDirection, true, true);
				ctx.LineTo(new Point(dx, dy), true, true);
				ctx.LineTo(new Point(xs, ys), true, true);
			}

			// create the drawing
			GeometryDrawing drawing = new GeometryDrawing();
			drawing.Geometry = streamGeom;
			return drawing;
		}
	}
}
