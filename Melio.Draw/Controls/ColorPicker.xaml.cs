using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Melio.Draw.Controls
{
	/// <summary>
	/// Interaktionslogik für ColorPicker.xaml
	/// </summary>
	/// http://stackoverflow.com/questions/4362536/wpf-gradient-in-2-directions
	public partial class ColorPicker : UserControl
	{
		public ColorPicker()
		{
			InitializeComponent();
		}

		#region Properties

		#region A

		public static readonly DependencyProperty AProperty = DependencyProperty.Register("A", typeof(byte), typeof(ColorPicker), new UIPropertyMetadata((byte)255, OnAChanged));
		public byte A
		{
			get
			{
				return (byte)GetValue(AProperty);
			}
			set
			{
				SetValue(AProperty, value);
			}
		}

		private static void OnAChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorPicker colorCanvas = o as ColorPicker;
			if (colorCanvas != null)
				colorCanvas.OnAChanged((byte)e.OldValue, (byte)e.NewValue);
		}

		protected virtual void OnAChanged(byte oldValue, byte newValue)
		{

		}

		#endregion //A

		#region R

		public static readonly DependencyProperty RProperty = DependencyProperty.Register("R", typeof(byte), typeof(ColorPicker), new UIPropertyMetadata((byte)0, OnRChanged));
		public byte R
		{
			get
			{
				return (byte)GetValue(RProperty);
			}
			set
			{
				SetValue(RProperty, value);
			}
		}

		private static void OnRChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorPicker colorCanvas = o as ColorPicker;
			if (colorCanvas != null)
				colorCanvas.OnRChanged((byte)e.OldValue, (byte)e.NewValue);
		}

		protected virtual void OnRChanged(byte oldValue, byte newValue)
		{

		}

		#endregion //R

		#region G

		public static readonly DependencyProperty GProperty = DependencyProperty.Register("G", typeof(byte), typeof(ColorPicker), new UIPropertyMetadata((byte)0, OnGChanged));
		public byte G
		{
			get
			{
				return (byte)GetValue(GProperty);
			}
			set
			{
				SetValue(GProperty, value);
			}
		}

		private static void OnGChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorPicker colorCanvas = o as ColorPicker;
			if (colorCanvas != null)
				colorCanvas.OnGChanged((byte)e.OldValue, (byte)e.NewValue);
		}

		protected virtual void OnGChanged(byte oldValue, byte newValue)
		{

		}

		#endregion //G

		#region B

		public static readonly DependencyProperty BProperty = DependencyProperty.Register("B", typeof(byte), typeof(ColorPicker), new UIPropertyMetadata((byte)0, OnBChanged));
		public byte B
		{
			get
			{
				return (byte)GetValue(BProperty);
			}
			set
			{
				SetValue(BProperty, value);
			}
		}

		private static void OnBChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorPicker colorCanvas = o as ColorPicker;
			if (colorCanvas != null)
				colorCanvas.OnBChanged((byte)e.OldValue, (byte)e.NewValue);
		}

		protected virtual void OnBChanged(byte oldValue, byte newValue)
		{

		}

		#endregion //B

		#endregion

	}
}
