using SharpDX;
using System;
using System.Windows;
using System.Windows.Media;

namespace Melio.Draw.ViewModel
{
	public class ColorVM : DependencyObject
	{
		public enum Channel
		{
			Red, Green, Blue,
			Hue, Saturation, Value,
			Alpha
		}

		#region private members
		//private float _red;
		//private float _green;
		//private float _blue;
		//private float _alpha;
		private Vector4 _rgba;
		private float _hue;
		private float _saturation;
		private float _value;
		//private SolidColorBrush _brush;
		#endregion


		#region Constructor

		public ColorVM(ColorVM source)
		{
			//_red = 0.0f;
			//_green = 0.0f;
			//_blue = 0.0f;
			//_alpha = 1.0f;
			_rgba = new Vector4(source.Red, source.Green, source.Blue, source.Alpha);
			updateHSV();
			Red = _rgba.X;
			Green = _rgba.Y;
			Blue = _rgba.Z;
			Alpha = _rgba.W;
			//_brush = new SolidColorBrush(new System.Windows.Media.Color()
			//{
			//	R = (byte)(_rgba.X * 255.0f),
			//	G = (byte)(_rgba.Y * 255.0f),
			//	B = (byte)(_rgba.Z * 255.0f),
			//	A = (byte)(_rgba.W * 255.0f)
			//});
			updateBrush();
		}

		public ColorVM()
		{
			//_red = 0.0f;
			//_green = 0.0f;
			//_blue = 0.0f;
			//_alpha = 1.0f;
			_rgba = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
			//_hue = 0.0f;
			//_saturation = 0.0f;
			//_value = 0.0f;
			updateHSV();
			Red = _rgba.X;
			Green = _rgba.Y;
			Blue = _rgba.Z;
			Alpha = _rgba.W;
			updateBrush();
		}

		public ColorVM(float red, float green, float blue)
		{
			//_red = red;
			//_green = green;
			//_blue = blue;
			//_alpha = 1.0f;
			_rgba = new Vector4(red, green, blue, 1.0f);
			updateHSV();
			Red = _rgba.X;
			Green = _rgba.Y;
			Blue = _rgba.Z;
			Alpha = _rgba.W;
			updateBrush();
			//_hue = 0.0f;
			//_saturation = 0.0f;
			//_value = 0.0f;
		}

		public ColorVM(float red, float green, float blue, float alpha)
		{
			//_red = red;
			//_green = green;
			//_blue = blue;
			//_alpha = alpha;
			_rgba = new Vector4(red, green, blue, alpha);
			updateHSV();
			Red = red;
			Green = green;
            Blue = blue;
			Alpha = alpha;
			updateBrush();
			//_hue = 0.0f;
			//_saturation = 0.0f;
			//_value = 0.0f;
		}
		#endregion
		#region Properties

		public float Red
		{
			get
			{
				return (float)GetValue(RedProperty);
			}
			set
			{
				SetValue(RedProperty, value);
			}
		}

		public float Green
		{
			get
			{
				return (float)GetValue(GreenProperty);
			}
			set
			{
				SetValue(GreenProperty, value);
			}
		}

		public float Blue
		{
			get
			{
				return (float)GetValue(BlueProperty);
			}
			set
			{
				SetValue(BlueProperty, value);
			}
		}

		public float Hue
		{
			get
			{
				return (float)GetValue(HueProperty);
			}
			set
			{
				SetValue(HueProperty, value);
			}
		}

		public float Saturation
		{
			get
			{
				return (float)GetValue(SaturationProperty);
			}
			set
			{
				SetValue(SaturationProperty, value);
			}
		}

		public float Value
		{
			get
			{
				return (float)GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);
			}
		}

		public float Alpha
		{
			get
			{
				return (float)GetValue(AlphaProperty);
			}
			set
			{
				SetValue(AlphaProperty, value);
			}
		}

		public SolidColorBrush Brush
		{
			get
			{
				return (SolidColorBrush)GetValue(BrushProperty);
			}
			protected set
			{
				SetValue(BrushProperty, value);
			}
		}

		public System.Windows.Media.Color WindowsColor
		{
			get
			{
				return new System.Windows.Media.Color() {
					R = (byte)(_rgba.X * 255.0f),
					G = (byte)(_rgba.Y * 255.0f),
					B = (byte)(_rgba.Z * 255.0f),
					A = (byte)(_rgba.W * 255.0f)
				};
			}
		}

		public void SetWindowsColor(ref System.Windows.Media.Color color)
		{
			color.R = (byte)(_rgba.X * 255.0f);
			color.G = (byte)(_rgba.Y * 255.0f);
			color.B = (byte)(_rgba.Z * 255.0f);
			color.A = (byte)(_rgba.W * 255.0f);
        }

		#endregion
		#region DependencyProperty
		public static readonly DependencyProperty RedProperty;
		public static readonly DependencyProperty GreenProperty;
		public static readonly DependencyProperty BlueProperty;
		public static readonly DependencyProperty ValueProperty;
		public static readonly DependencyProperty HueProperty;
		public static readonly DependencyProperty SaturationProperty;
		public static readonly DependencyProperty AlphaProperty;
		public static readonly DependencyProperty BrushProperty;


		private static void OnRedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorVM color = o as ColorVM;
			if (color != null)
				color.OnRedChanged((float)e.OldValue, (float)e.NewValue);
		}

		private static void OnGreenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorVM colorCanvas = o as ColorVM;
			if (colorCanvas != null)
				colorCanvas.OnGreenChanged((float)e.OldValue, (float)e.NewValue);
		}

		private static void OnBlueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorVM colorCanvas = o as ColorVM;
			if (colorCanvas != null)
				colorCanvas.OnBlueChanged((float)e.OldValue, (float)e.NewValue);
		}

		private static void OnHueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorVM colorCanvas = o as ColorVM;
			if (colorCanvas != null)
				colorCanvas.OnHueChanged((float)e.OldValue, (float)e.NewValue);
		}

		private static void OnSaturationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorVM colorCanvas = o as ColorVM;
			if (colorCanvas != null)
				colorCanvas.OnSaturationChanged((float)e.OldValue, (float)e.NewValue);
		}

		private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorVM colorCanvas = o as ColorVM;
			if (colorCanvas != null)
				colorCanvas.OnValueChanged((float)e.OldValue, (float)e.NewValue);
		}

		private static void OnAlphaChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorVM colorCanvas = o as ColorVM;
			if (colorCanvas != null)
				colorCanvas.OnAlphaChanged((float)e.OldValue, (float)e.NewValue);
		}

		private static void OnBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorVM colorCanvas = o as ColorVM;
			if (colorCanvas != null)
				colorCanvas.OnBrushChanged((SolidColorBrush)e.OldValue, (SolidColorBrush)e.NewValue);
		}

		static ColorVM()
		{
			RedProperty = DependencyProperty.Register("Red", typeof(float), typeof(ColorVM), new UIPropertyMetadata(0.0f, OnRedChanged));
			GreenProperty = DependencyProperty.Register("Green", typeof(float), typeof(ColorVM), new UIPropertyMetadata(0.0f, OnGreenChanged));
			BlueProperty = DependencyProperty.Register("Blue", typeof(float), typeof(ColorVM), new UIPropertyMetadata(0.0f, OnBlueChanged));
			HueProperty = DependencyProperty.Register("Hue", typeof(float), typeof(ColorVM), new UIPropertyMetadata(0.0f, OnHueChanged));
			SaturationProperty = DependencyProperty.Register("Saturation", typeof(float), typeof(ColorVM), new UIPropertyMetadata(0.0f, OnSaturationChanged));
			ValueProperty = DependencyProperty.Register("Value", typeof(float), typeof(ColorVM), new UIPropertyMetadata(0.0f, OnValueChanged));
			AlphaProperty = DependencyProperty.Register("Alpha", typeof(float), typeof(ColorVM), new UIPropertyMetadata(1.0f, OnAlphaChanged));
			BrushProperty = DependencyProperty.Register("Brush", typeof(SolidColorBrush), typeof(ColorVM), new UIPropertyMetadata(new SolidColorBrush() { Color = Colors.Black }, OnBrushChanged));
			//BrushProperty = DependencyProperty.Register("Brush", typeof(SolidColorBrush), typeof(ColorVM), new UIPropertyMetadata(null));
		}
		#endregion
		#region On Cnanged Events
		protected void OnRedChanged(float oldValue, float newValue)
		{
			if(newValue != _rgba.X)
			{
				_rgba.X = newValue;
				updateHSV();
				updateBrush();
			}
		}

		protected void OnGreenChanged(float oldValue, float newValue)
		{
			if (newValue != _rgba.Y)
			{
				_rgba.Y = newValue;
				updateBrush();
				updateHSV();
			}
		}

		protected void OnBlueChanged(float oldValue, float newValue)
		{
			if (newValue != _rgba.Z)
			{
				_rgba.Z = newValue;
				updateHSV();
				updateBrush();
			}
		}

		protected void OnHueChanged(float oldValue, float newValue)
		{
			if (newValue != _hue)
			{
				_hue = newValue;
				updateRGB();
				updateBrush();
			}
		}

		protected void OnSaturationChanged(float oldValue, float newValue)
		{
			if (newValue != _saturation)
			{
				_saturation = newValue;
				updateRGB();
				updateBrush();
			}
		}

		protected void OnValueChanged(float oldValue, float newValue)
		{
			if (newValue != _value)
			{
				_value = newValue;
				updateRGB();
				updateBrush();
			}
		}

		protected void OnAlphaChanged(float oldValue, float newValue)
		{
			if (newValue != _rgba.W)
			{
				_rgba.W = newValue;
				updateBrush();
            }
		}

		protected void OnBrushChanged(SolidColorBrush oldValue, SolidColorBrush newValue)
		{
		}
		#endregion
		#region public functions
		public static void HSVtoRGB(float h, float s, float v, out float r, out float g, out float b)
		{
			int hi = (int)Math.Floor(h/60.0f);
			float f = h / 60.0f - hi;
			float p = v * (1.00f - s);
			float q = v * (1.0f - s * f);
			float t = v * (1-s*(1.0f-f));

			switch (hi)
			{
				case 0:
				case 6:
					r = v;
					g = t;
					b = p;
					break;
				case 1:
					r = q;
					g = v;
					b = p;
					break;
				case 2:
					r = p;
					g = v;
					b = t;
					break;
				case 3:
					r = p;
					g = q;
					b = v;
					break;
				case 4:
					r = t;
					g = p;
					b = v;
					break;
				case 5:
					r = v;
					g = p;
					b = q;
					break;
				default:
					r = 0.0f;
					g = 0.0f;
					b = 0.0f;
					break;
			}

		}
		public static void RGBtoHSV(float r, float g, float b, out float h, out float s, out float v)
		{
			float max = Math.Max(r, Math.Max(g, b));
			float min = Math.Min(r, Math.Min(g, b));

			if (max == min)
				h = 0.0f;
			else if (max == r)
				h = 60.0f * (0.0f + (g - b) / (max - min));
			else if (max == g)
				h = 60.0f * (2.0f + (b - r) / (max - min));
			else if (max == b)
				h = 60.0f * (4.0f + (r - g) / (max - min));
			else
				h = 0.0f;

			if (h < 0.0f)
				h += 360.0f;

			if (max == 0.0f)
				s = 0.0f;
			else
				s = (max-min) / max;

			v = max;
		}

		public Vector4 Vector4
		{
			//get { return new Vector4(_red, _green, _blue, _alpha); }
			get { return _rgba; }
		}

		public Vector3 Vector3
		{
			get { return new Vector3(_rgba.X, _rgba.Y, _rgba.Z); }
		}

		public void FillVector(ref Vector4 vector)
		{
			//vector.X = _red;
			//vector.Y = _green;
			//vector.Z = _blue;
			//vector.W = _alpha;
			vector = _rgba;
        }

		public void FillVector(ref Vector3 vector)
		{
			//vector.X = _red;
			//vector.Y = _green;
			//vector.Z = _blue;
			vector.X = _rgba.X;
			vector.Y = _rgba.Y;
			vector.Z = _rgba.Z;
		}
		#endregion
		#region private functions
		private void updateHSV()
		{
			//RGBtoHSV(_red, _green, _blue, out _hue, out _saturation, out _value);
			RGBtoHSV(_rgba.X, _rgba.Y, _rgba.Z, out _hue, out _saturation, out _value);
			Hue = _hue;
			Saturation = _saturation;
			Value = _value;
		}
		private void updateRGB()
		{
			//HSVtoRGB(_hue, _saturation, _value, out _red, out _green, out _blue);
			HSVtoRGB(_hue, _saturation, _value, out _rgba.X, out _rgba.Y, out _rgba.Z);
			Red = _rgba.X;
			Green = _rgba.Y;
			Blue = _rgba.Z;
		}
		private void updateBrush()
		{
			Brush = new SolidColorBrush()
			{
				Color = System.Windows.Media.Color.FromArgb(
					(byte)(_rgba.W * 255.0f),
					(byte)(_rgba.X * 255.0f),
					(byte)(_rgba.Y * 255.0f),
					(byte)(_rgba.Z * 255.0f))
			};
		}
		#endregion

	}
}
