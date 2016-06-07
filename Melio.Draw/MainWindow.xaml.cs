using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Fluent;
using SharpDX;

using Point = System.Windows.Point;

namespace Melio.Draw
{
	// http://wpftutorial.net/UndoRedo.html
	#region Converter
	public class PointToStringConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Point))
				return "---";
			var p = (Point)value;
			return string.Format(culture, "{0:n0};{1:n0}", p.X, p.Y);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}
	public class Vector2ToStringConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Vector2))
				return "---";
			var p = (Vector2)value;
			return string.Format(culture, "{0:n0};{1:n0}", p.X, p.Y);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}
	public class FloatToStringConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is float))
				return "---";
			var p = (float)value;
			return string.Format(culture, "{0:n1}%", p*100);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}
	#endregion
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}
	}
}
