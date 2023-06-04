using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FileSizeVisualizer2
{
	public class FilePathIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string _path)
			{
				Icon icon;

				if (!Directory.Exists(_path) && System.Drawing.Icon.ExtractAssociatedIcon(_path) is Icon _icon)
				{
					icon = _icon;
				}
				else
				{
					icon = DefaultIcons.FolderLarge;
				}

				MemoryStream strm = new();
				icon.Save(strm);
				IconBitmapDecoder ibd = new(strm, BitmapCreateOptions.None, BitmapCacheOption.None);
				return ibd.Frames[0];
			}
			else
			{
				throw new InvalidCastException();
			}
		}

		public object ConvertBack(object value, Type targetType, object paramter, string language)
		{
			throw new InvalidCastException("One to many operation");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidCastException("One to many operation");
		}
	}
}
