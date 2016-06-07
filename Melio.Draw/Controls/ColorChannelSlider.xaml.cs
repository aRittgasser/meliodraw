using Melio.Draw.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaktionslogik für ColorChannelSlider.xaml
	/// </summary>
	public partial class ColorChannelSlider : UserControl
	{
		public static readonly DependencyProperty ColorProperty;
		public static readonly DependencyProperty ChannelProperty;

		private ColorVM _tmpColor;

		public ColorVM Color
		{
			get
			{
				return (ColorVM)GetValue(ColorProperty);
            }
			set
			{
				SetValue(ColorProperty, value);
			}
		}

		public ColorVM.Channel Channel
		{
			get
			{
				return (ColorVM.Channel)GetValue(ChannelProperty);
			}
			set
			{
				SetValue(ChannelProperty, value);
			}
		}


		private static void OnChannelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorChannelSlider colorSlider = o as ColorChannelSlider;
			if (colorSlider != null)
				colorSlider.OnChannelChanged((ColorVM.Channel)e.OldValue, (ColorVM.Channel)e.NewValue);
		}

		private static void OnColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ColorChannelSlider colorSlider = o as ColorChannelSlider;
			if (colorSlider != null)
				colorSlider.OnColorChanged((ColorVM)e.OldValue, (ColorVM)e.NewValue);
		}


		static ColorChannelSlider()
		{
			ColorProperty = DependencyProperty.Register("Color", typeof(ColorVM), typeof(ColorChannelSlider), new UIPropertyMetadata(OnColorChanged));
			ChannelProperty = DependencyProperty.Register("Channel", typeof(ColorVM.Channel), typeof(ColorChannelSlider), new UIPropertyMetadata(ColorVM.Channel.Red, OnChannelChanged));
		}

		public ColorChannelSlider()
		{
			InitializeComponent();
			_tmpColor = new ColorVM();

		}
		#region Events
		private void OnChannelChanged(ColorVM.Channel oldValue, ColorVM.Channel newValue)
		{
			setColorSteps();
			switch (Channel)
			{
				case ColorVM.Channel.Red:
					_slider.Value = _tmpColor.Red;
					break;
				case ColorVM.Channel.Green:
					_slider.Value = _tmpColor.Green;
					break;
				case ColorVM.Channel.Blue:
					_slider.Value = _tmpColor.Blue;
					break;
				case ColorVM.Channel.Alpha:
					_slider.Value = _tmpColor.Alpha;
					break;
				case ColorVM.Channel.Saturation:
					_slider.Value = _tmpColor.Saturation;
					break;
				case ColorVM.Channel.Value:
					_slider.Value = _tmpColor.Value;
					break;
				case ColorVM.Channel.Hue:
					_slider.Value = _tmpColor.Hue;
					break;
			}
		}

		private void OnColorChanged(ColorVM oldValue, ColorVM newValue)
		{
			if (newValue == null)
				return;
			DependencyPropertyDescriptor.FromProperty(ColorVM.RedProperty, typeof(ColorVM)).AddValueChanged(newValue, (s,e)=> {
				_tmpColor.Red = (s as ColorVM).Red;
				if (Channel == ColorVM.Channel.Red)
					_slider.Value = _tmpColor.Red;
				else
					setColorSteps();
			});
			DependencyPropertyDescriptor.FromProperty(ColorVM.GreenProperty, typeof(ColorVM)).AddValueChanged(newValue, (s, e) => {
				_tmpColor.Green = (s as ColorVM).Green;
				if (Channel == ColorVM.Channel.Green)
					_slider.Value = _tmpColor.Green;
				else
					setColorSteps();
			});
			DependencyPropertyDescriptor.FromProperty(ColorVM.BlueProperty, typeof(ColorVM)).AddValueChanged(newValue, (s, e) => {
				_tmpColor.Blue = (s as ColorVM).Blue;
				if (Channel == ColorVM.Channel.Blue)
					_slider.Value = _tmpColor.Blue;
				else
					setColorSteps();
			});
			DependencyPropertyDescriptor.FromProperty(ColorVM.AlphaProperty, typeof(ColorVM)).AddValueChanged(newValue, (s, e) => {
				_tmpColor.Alpha = (s as ColorVM).Alpha;
				if (Channel == ColorVM.Channel.Alpha)
					_slider.Value = _tmpColor.Alpha;
				else
					setColorSteps();
			});
			DependencyPropertyDescriptor.FromProperty(ColorVM.SaturationProperty, typeof(ColorVM)).AddValueChanged(newValue, (s, e) => {
				_tmpColor.Saturation = (s as ColorVM).Saturation;
				if (Channel == ColorVM.Channel.Saturation)
					_slider.Value = _tmpColor.Saturation;
				else
					setColorSteps();
			});
			DependencyPropertyDescriptor.FromProperty(ColorVM.ValueProperty, typeof(ColorVM)).AddValueChanged(newValue, (s, e) => {
				_tmpColor.Value = (s as ColorVM).Value;
				if (Channel == ColorVM.Channel.Value)
					_slider.Value = _tmpColor.Value;
				else
					setColorSteps();
			});
			DependencyPropertyDescriptor.FromProperty(ColorVM.HueProperty, typeof(ColorVM)).AddValueChanged(newValue, (s, e) => {
				_tmpColor.Hue = (s as ColorVM).Hue;
				if (Channel == ColorVM.Channel.Hue)
					_slider.Value = _tmpColor.Hue/360.0f;
				else
					setColorSteps();
			});
			_tmpColor.Red = newValue.Red;
			_tmpColor.Green = newValue.Green;
			_tmpColor.Blue = newValue.Blue;
			_tmpColor.Alpha = newValue.Alpha;
			switch (Channel)
			{
				case ColorVM.Channel.Red:
					_slider.Value = _tmpColor.Red;
					break;
				case ColorVM.Channel.Green:
					_slider.Value = _tmpColor.Green;
					break;
				case ColorVM.Channel.Blue:
					_slider.Value = _tmpColor.Blue;
					break;
				case ColorVM.Channel.Alpha:
					_slider.Value = _tmpColor.Alpha;
					break;
				case ColorVM.Channel.Saturation:
					_slider.Value = _tmpColor.Saturation;
					break;
				case ColorVM.Channel.Value:
					_slider.Value = _tmpColor.Value;
					break;
				case ColorVM.Channel.Hue:
					_slider.Value = _tmpColor.Hue;
					break;
			}
			//         _tmpColor.Red = newValue.Red;
			//_tmpColor.Green = newValue.Green;
			//_tmpColor.Blue = newValue.Blue;
			//_tmpColor.Alpha = newValue.Alpha;
			//_textBox.Text = _tmpColor.Red.ToString();
			//setColorSteps();
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Color == null)
				return;
			switch (Channel)
			{
				case ColorVM.Channel.Red:
					_tmpColor.Red = (float)e.NewValue;
					_textBox.Text = _tmpColor.Red.ToString();
                    Color.Red = _tmpColor.Red;
                    break;
				case ColorVM.Channel.Green:
					_tmpColor.Green = (float)e.NewValue;
					_textBox.Text = _tmpColor.Green.ToString();
					Color.Green = _tmpColor.Green;
					break;
				case ColorVM.Channel.Blue:
					_tmpColor.Blue = (float)e.NewValue;
					_textBox.Text = _tmpColor.Blue.ToString();
					Color.Blue = _tmpColor.Blue;
					break;
				case ColorVM.Channel.Alpha:
					_tmpColor.Alpha = (float)e.NewValue;
					_textBox.Text = _tmpColor.Alpha.ToString();
					Color.Alpha = _tmpColor.Alpha;
					break;
				case ColorVM.Channel.Saturation:
					_tmpColor.Saturation = (float)e.NewValue;
					_textBox.Text = _tmpColor.Saturation.ToString();
					Color.Saturation = _tmpColor.Saturation;
					break;
				case ColorVM.Channel.Value:
					_tmpColor.Value = (float)e.NewValue;
					_textBox.Text = _tmpColor.Value.ToString();
					Color.Value = _tmpColor.Value;
					break;
				case ColorVM.Channel.Hue:
					_tmpColor.Hue = (float)e.NewValue*360.0f;
					_textBox.Text = _tmpColor.Hue.ToString();
					Color.Hue = _tmpColor.Hue;
					break;

			}
		}
		private void setColorSteps()
		{
			switch (Channel)
			{
				case ColorVM.Channel.Red:
					_tmpColor.Red = 0.0f;
					_color0.Color = _tmpColor.WindowsColor;
					_tmpColor.Red = 1.0f/6.0f;
					_color1.Color = _tmpColor.WindowsColor;
					_tmpColor.Red = 2.0f / 6.0f;
					_color2.Color = _tmpColor.WindowsColor;
					_tmpColor.Red = 3.0f / 6.0f;
					_color3.Color = _tmpColor.WindowsColor;
					_tmpColor.Red = 4.0f / 6.0f;
					_color4.Color = _tmpColor.WindowsColor;
					_tmpColor.Red = 5.0f / 6.0f;
					_color5.Color = _tmpColor.WindowsColor;
					_tmpColor.Red = 6.0f / 6.0f;
					_color6.Color = _tmpColor.WindowsColor;
					_textBox.Text = _tmpColor.Red.ToString();
					break;
				case ColorVM.Channel.Green:
					_tmpColor.Green = 0.0f;
					_color0.Color = _tmpColor.WindowsColor;
					_tmpColor.Green = 1.0f / 6.0f;
					_color1.Color = _tmpColor.WindowsColor;
					_tmpColor.Green = 2.0f / 6.0f;
					_color2.Color = _tmpColor.WindowsColor;
					_tmpColor.Green = 3.0f / 6.0f;
					_color3.Color = _tmpColor.WindowsColor;
					_tmpColor.Green = 4.0f / 6.0f;
					_color4.Color = _tmpColor.WindowsColor;
					_tmpColor.Green = 5.0f / 6.0f;
					_color5.Color = _tmpColor.WindowsColor;
					_tmpColor.Green = 6.0f / 6.0f;
					_color6.Color = _tmpColor.WindowsColor;
					_textBox.Text = _tmpColor.Green.ToString();
					break;
				case ColorVM.Channel.Blue:
					_tmpColor.Blue = 0.0f;
					_color0.Color = _tmpColor.WindowsColor;
					_tmpColor.Blue = 1.0f / 6.0f;
					_color1.Color = _tmpColor.WindowsColor;
					_tmpColor.Blue = 2.0f / 6.0f;
					_color2.Color = _tmpColor.WindowsColor;
					_tmpColor.Blue = 3.0f / 6.0f;
					_color3.Color = _tmpColor.WindowsColor;
					_tmpColor.Blue = 4.0f / 6.0f;
					_color4.Color = _tmpColor.WindowsColor;
					_tmpColor.Blue = 5.0f / 6.0f;
					_color5.Color = _tmpColor.WindowsColor;
					_tmpColor.Blue = 6.0f / 6.0f;
					_color6.Color = _tmpColor.WindowsColor;
					_textBox.Text = _tmpColor.Blue.ToString();
					break;
				case ColorVM.Channel.Alpha:
					_tmpColor.Alpha = 0.0f;
					_color0.Color = _tmpColor.WindowsColor;
					_tmpColor.Alpha = 1.0f / 6.0f;
					_color1.Color = _tmpColor.WindowsColor;
					_tmpColor.Alpha = 2.0f / 6.0f;
					_color2.Color = _tmpColor.WindowsColor;
					_tmpColor.Alpha = 3.0f / 6.0f;
					_color3.Color = _tmpColor.WindowsColor;
					_tmpColor.Alpha = 4.0f / 6.0f;
					_color4.Color = _tmpColor.WindowsColor;
					_tmpColor.Alpha = 5.0f / 6.0f;
					_color5.Color = _tmpColor.WindowsColor;
					_tmpColor.Alpha = 6.0f / 6.0f;
					_color6.Color = _tmpColor.WindowsColor;
					_textBox.Text = _tmpColor.Alpha.ToString();
					break;
				case ColorVM.Channel.Saturation:
					_tmpColor.Saturation = 0.0f;
					_color0.Color = _tmpColor.WindowsColor;
					_tmpColor.Saturation = 1.0f / 6.0f;
					_color1.Color = _tmpColor.WindowsColor;
					_tmpColor.Saturation = 2.0f / 6.0f;
					_color2.Color = _tmpColor.WindowsColor;
					_tmpColor.Saturation = 3.0f / 6.0f;
					_color3.Color = _tmpColor.WindowsColor;
					_tmpColor.Saturation = 4.0f / 6.0f;
					_color4.Color = _tmpColor.WindowsColor;
					_tmpColor.Saturation = 5.0f / 6.0f;
					_color5.Color = _tmpColor.WindowsColor;
					_tmpColor.Saturation = 6.0f / 6.0f;
					_color6.Color = _tmpColor.WindowsColor;
					_textBox.Text = _tmpColor.Saturation.ToString();
					break;
				case ColorVM.Channel.Value:
					_tmpColor.Value = 0.0f;
					_color0.Color = _tmpColor.WindowsColor;
					_tmpColor.Value = 1.0f / 6.0f;
					_color1.Color = _tmpColor.WindowsColor;
					_tmpColor.Value = 2.0f / 6.0f;
					_color2.Color = _tmpColor.WindowsColor;
					_tmpColor.Value = 3.0f / 6.0f;
					_color3.Color = _tmpColor.WindowsColor;
					_tmpColor.Value = 4.0f / 6.0f;
					_color4.Color = _tmpColor.WindowsColor;
					_tmpColor.Value = 5.0f / 6.0f;
					_color5.Color = _tmpColor.WindowsColor;
					_tmpColor.Value = 6.0f / 6.0f;
					_color6.Color = _tmpColor.WindowsColor;
					_textBox.Text = _tmpColor.Value.ToString();
					break;
				case ColorVM.Channel.Hue:
					_tmpColor.Hue = 0.0f;
					_color0.Color = _tmpColor.WindowsColor;
					_tmpColor.Hue = 1.0f * 60.0f;
					_color1.Color = _tmpColor.WindowsColor;
					_tmpColor.Hue = 2.0f * 60.0f;
					_color2.Color = _tmpColor.WindowsColor;
					_tmpColor.Hue = 3.0f * 60.0f;
					_color3.Color = _tmpColor.WindowsColor;
					_tmpColor.Hue = 4.0f * 60.0f;
					_color4.Color = _tmpColor.WindowsColor;
					_tmpColor.Hue = 5.0f * 60.0f;
					_color5.Color = _tmpColor.WindowsColor;
					_tmpColor.Hue = 6.0f * 60.0f;
					_color6.Color = _tmpColor.WindowsColor;
					_textBox.Text = _tmpColor.Hue.ToString();
					break;
			}
		}
		#endregion
	}
}
