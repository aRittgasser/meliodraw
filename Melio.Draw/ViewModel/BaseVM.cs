using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Melio.Draw.ViewModel
{

	public abstract class BaseVM
		: DependencyObject
	{

		#region Eigenschaften

		protected bool IsInDesignMode { get { return DesignerProperties.GetIsInDesignMode(this); } }

		#endregion

		#region Ereignisse

		public event EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;

		#endregion

		#region ctor

		protected BaseVM()
		{
		}

		#endregion

		#region Funktionen

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, e);
			base.OnPropertyChanged(e);
		}

		//protected override void OnPropertyChanged([CallerMemberName]string name = null)
		//{
		//	var e = new PropertyChangedEventArgs(name);
  //          var handler = PropertyChanged;
		//	if (handler != null)
		//		handler(this, e);
		//	base.OnPropertyChanged(e);
		//}

		protected string GetFunctionName([CallerMemberName]string name = null)
		{
			return name;
		}

		#endregion

	}

	public abstract class DisplayNameBaseVM
		: BaseVM
	{

		#region Variablen

		public static readonly DependencyProperty DisplayNameProperty;

		#endregion

		#region Eigenschaften

		public string DisplayName { get { return GetValue(DisplayNameProperty) as string; } set { SetValue(DisplayNameProperty, value); } }

		#endregion

		#region ctor

		static DisplayNameBaseVM()
		{
			DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(DisplayNameBaseVM), new PropertyMetadata(null, DisplayNamePropertyChanged));
		}

		protected DisplayNameBaseVM(string displayName)
			: base()
		{
			if (displayName == null)
				throw new ArgumentNullException("displayName", "DisplayName is null.");
			DisplayName = displayName;
		}

		#endregion

		#region Ereignishandler

		private static void DisplayNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == null)
				throw new InvalidOperationException("DisplayNameProperty must not null.");
		}

		#endregion

	}

	public abstract class IndexBaseVM
		: BaseVM
	{

		#region Variablen

		public static readonly DependencyProperty IndexProperty;

		#endregion

		#region Eigenschaften

		public int Index { get { return (int)GetValue(IndexProperty); } set { SetValue(IndexProperty, value); } }

		#endregion

		#region ctor

		static IndexBaseVM()
		{
			IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(IndexBaseVM), new PropertyMetadata(0));
		}

		protected IndexBaseVM(int index)
		{
			Index = index;
		}

		#endregion

	}

}
