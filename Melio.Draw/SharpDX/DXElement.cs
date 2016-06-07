using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpDX;
using Point = System.Windows.Point;

namespace Melio.Draw.SharpDX
{

	public class DXElement
		: FrameworkElement
	{

		#region Variablen

		public static readonly DependencyProperty MousePositionProperty;
		private static readonly DependencyPropertyKey MousePositionPropertyKey;
		public static readonly DependencyProperty IsMouseLeftButtonPressedProperty;
		private static readonly DependencyPropertyKey IsMouseLeftButtonPressedPropertyKey;
		public static readonly DependencyProperty IsMouseMiddleButtonPressedProperty;
		private static readonly DependencyPropertyKey IsMouseMiddleButtonPressedPropertyKey;
		public static readonly DependencyProperty IsMouseRightButtonPressedProperty;
		private static readonly DependencyPropertyKey IsMouseRightButtonPressedPropertyKey;
		public static readonly DependencyProperty IsMouseXButton1PressedProperty;
		private static readonly DependencyPropertyKey IsMouseXButton1PressedPropertyKey;
		public static readonly DependencyProperty IsMouseXButton2PressedProperty;
		private static readonly DependencyPropertyKey IsMouseXButton2PressedPropertyKey;
		public static readonly DependencyProperty FPSProperty;
		private static readonly DependencyPropertyKey FPSPropertyKey;
		public static readonly DependencyProperty IsLoopRenderingProperty;
		public static readonly DependencyProperty RendererProperty;

		private bool isReallyLoopRendering;
		private DrawEventArgs lastDrawEventArgs;
		private TimerTick renderTimer;
		private TimeSpan lastFpsUpdate;
		private int fpsFrameCounter;
		private WriteableBitmap surface;
		// Cache für Größenberechnung.
		// Mit diesem Hack bin ich in der Lage, InvalidateVisual aufzurufen, ohne dass die Größe neu berechnet werden muss.
		// Dies würde nämlich ein Neuerstellen das BackgroundImages (surface) auslösen, was fatale Folgen für den Ram hat. (60x / Sekunde...)
		private Size? arrangeResult;

		#endregion

		#region Eigenschaften

		protected bool IsInDesignMode { get { return DesignerProperties.GetIsInDesignMode(this); } }

		public Vector2? MousePosition { get { return (Vector2?)GetValue(MousePositionProperty); } private set { SetValue(MousePositionPropertyKey, value); } }
		public bool IsMouseLeftButtonPressed { get { return (bool)GetValue(IsMouseLeftButtonPressedProperty); } private set { SetValue(IsMouseLeftButtonPressedPropertyKey, value); } }
		public bool IsMouseMiddleButtonPressed { get { return (bool)GetValue(IsMouseMiddleButtonPressedProperty); } private set { SetValue(IsMouseMiddleButtonPressedPropertyKey, value); } }
		public bool IsMouseRightButtonPressed { get { return (bool)GetValue(IsMouseRightButtonPressedProperty); } private set { SetValue(IsMouseRightButtonPressedPropertyKey, value); } }
		public bool IsMouseXButton1Pressed { get { return (bool)GetValue(IsMouseXButton1PressedProperty); } private set { SetValue(IsMouseXButton1PressedPropertyKey, value); } }
		public bool IsMouseXButton2Pressed { get { return (bool)GetValue(IsMouseXButton2PressedProperty); } private set { SetValue(IsMouseXButton2PressedPropertyKey, value); } }
		public double FPS { get { return (double)GetValue(FPSProperty); } private set { SetValue(FPSPropertyKey, value); } }
		public bool IsLoopRendering { get { return (bool)GetValue(IsLoopRenderingProperty); } set { SetValue(IsLoopRenderingProperty, value); } }
		public IDirect3D Renderer { get { return GetValue(RendererProperty) as IDirect3D; } set { SetValue(RendererProperty, value); } }
		protected override int VisualChildrenCount { get { return 0; } }

		#endregion

		#region ctor

		static DXElement()
		{
			MousePositionPropertyKey = DependencyProperty.RegisterReadOnly("MousePosition", typeof(Vector2?), typeof(DXElement), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
			MousePositionProperty = MousePositionPropertyKey.DependencyProperty;
			IsMouseLeftButtonPressedPropertyKey = DependencyProperty.RegisterReadOnly("IsMouseLeftButtonPressed", typeof(bool), typeof(DXElement), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
			IsMouseLeftButtonPressedProperty = IsMouseLeftButtonPressedPropertyKey.DependencyProperty;
			IsMouseMiddleButtonPressedPropertyKey = DependencyProperty.RegisterReadOnly("IsMouseMiddleButtonPressed", typeof(bool), typeof(DXElement), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
			IsMouseMiddleButtonPressedProperty = IsMouseMiddleButtonPressedPropertyKey.DependencyProperty;
			IsMouseRightButtonPressedPropertyKey = DependencyProperty.RegisterReadOnly("IsMouseRightButtonPressed", typeof(bool), typeof(DXElement), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
			IsMouseRightButtonPressedProperty = IsMouseRightButtonPressedPropertyKey.DependencyProperty;
			IsMouseXButton1PressedPropertyKey = DependencyProperty.RegisterReadOnly("IsMouseXButton1Pressed", typeof(bool), typeof(DXElement), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
			IsMouseXButton1PressedProperty = IsMouseXButton1PressedPropertyKey.DependencyProperty;
			IsMouseXButton2PressedPropertyKey = DependencyProperty.RegisterReadOnly("IsMouseXButton2Pressed", typeof(bool), typeof(DXElement), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
			IsMouseXButton2PressedProperty = IsMouseXButton2PressedPropertyKey.DependencyProperty;
			FPSPropertyKey = DependencyProperty.RegisterReadOnly("FPS", typeof(double), typeof(DXElement), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
			FPSProperty = FPSPropertyKey.DependencyProperty;
			IsLoopRenderingProperty = DependencyProperty.Register("IsLoopRendering", typeof(bool), typeof(DXElement),
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsLoopRenderingChanged));
			RendererProperty = DependencyProperty.Register("Renderer", typeof(IDirect3D), typeof(DXElement), new PropertyMetadata(OnRendererChanged));
		}

		public DXElement()
		{
			SnapsToDevicePixels = true;
			renderTimer = new TimerTick();
			IsVisibleChanged += DXElement_IsVisibleChanged;

			if (IsInDesignMode)
				return;

			MouseEnter += DXElement_MouseEnter;
			MouseDown += DXElement_MouseDown;
			MouseMove += DXElement_MouseMove;
			MouseUp += DXElement_MouseUp;
			MouseWheel += DXElement_MouseWheel;
            MouseLeave += DXElement_MouseLeave;
			PreviewMouseMove += DXElement_PreviewMouseMove;
			PreviewMouseWheel += DXElement_PreviewMouseWheel;

			KeyDown += DXElement_KeyDown;
			KeyUp += DXElement_KeyUp;
		}

		#endregion

		#region Ereignishandler

		private static void IsLoopRenderingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var dxElement = d as DXElement;
			if (dxElement == null)
				return;
			if (e.OldValue is bool && e.NewValue is bool && (bool)e.OldValue == (bool)e.NewValue)
				return;
			dxElement.UpdateReallyLoopRendering();
		}

		private void OnLoopRendering(object sender, EventArgs e)
		{
			if (!isReallyLoopRendering)
				return;
			Render();
		}

		private static void OnRendererChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var dxElement = d as DXElement;
			if (dxElement == null)
				return;
			dxElement.OnRendererChanged(e.OldValue as IDirect3D, e.NewValue as IDirect3D);
		}

		private void OnRendererChanged(IDirect3D oldValue, IDirect3D newValue)
		{
			UpdateSize();
			Focusable = false;
		}

		private void DXElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			UpdateReallyLoopRendering();
		}

		private void DXElement_MouseEnter(object sender, MouseEventArgs e)
		{
			MousePosition = Renderer.Camera.UnProject(e.GetPosition(this));
		}

		private void DXElement_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) IsMouseLeftButtonPressed = true;
			else if (e.ChangedButton == MouseButton.Middle) IsMouseMiddleButtonPressed = true;
			else if (e.ChangedButton == MouseButton.Right) IsMouseRightButtonPressed = true;
			else if (e.ChangedButton == MouseButton.XButton1) IsMouseXButton1Pressed = true;
			else if (e.ChangedButton == MouseButton.XButton2) IsMouseXButton2Pressed = true;
			if (!IsMouseCaptured) Mouse.Capture(sender as IInputElement, CaptureMode.SubTree);
			Focus();
		}

		private void DXElement_MouseMove(object sender, MouseEventArgs e)
		{
			MousePosition = Renderer.Camera.UnProject(e.GetPosition(this));
		}
		private void DXElement_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			//MousePosition = Renderer.Camera.UnProject(e.GetPosition(this));
		}


		private void DXElement_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) IsMouseLeftButtonPressed = false;
			else if (e.ChangedButton == MouseButton.Middle) IsMouseMiddleButtonPressed = false;
			else if (e.ChangedButton == MouseButton.Right) IsMouseRightButtonPressed = false;
			else if (e.ChangedButton == MouseButton.XButton1) IsMouseXButton1Pressed = false;
			else if (e.ChangedButton == MouseButton.XButton2) IsMouseXButton2Pressed = false;
			if (e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && e.XButton1 == MouseButtonState.Released && e.XButton2 == MouseButtonState.Released)
				Mouse.Capture(null);
		}

		private void DXElement_MouseLeave(object sender, MouseEventArgs e)
		{
			MousePosition = null;
		}

		private void DXElement_MouseWheel(object sender, MouseEventArgs e)
		{
		}

		private void DXElement_PreviewMouseWheel(object sender, MouseEventArgs e)
		{
		}

		private void DXElement_KeyDown(object sender, KeyboardEventArgs e)
		{

		}

		private void DXElement_KeyUp(object sender, KeyboardEventArgs e)
		{

		}

		#endregion

		#region Funktionen

		protected override Size ArrangeOverride(Size finalSize)
		{
			//Debug.WriteLine("ArrangeOverride");
			if (!arrangeResult.HasValue)
			{
				base.ArrangeOverride(finalSize);
				UpdateSize();
				arrangeResult = finalSize;
			}
			return arrangeResult.Value;
		}

		public DrawEventArgs GetDrawEventArgs()
		{
			lastDrawEventArgs = new DrawEventArgs(renderTimer, DesiredSize, surface);
			return lastDrawEventArgs;
		}

		protected override Visual GetVisualChild(int index)
		{
			throw new ArgumentOutOfRangeException();
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			//Debug.WriteLine("MeasureOverride");
			arrangeResult = null;
			int w = (int)Math.Ceiling(availableSize.Width);
			int h = (int)Math.Ceiling(availableSize.Height);
			return new Size(w, h);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (IsInDesignMode)
				return;
			//Debug.WriteLine("OnRender");
			var size = DesiredSize;
			var rect = new Rect(0, 0, (int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
			drawingContext.DrawImage(surface, rect);
		}

		protected override void OnVisualParentChanged(DependencyObject oldParent)
		{
			if (IsInDesignMode)
				return;
			UpdateReallyLoopRendering();
		}

		public void Render()
		{
			if (Renderer == null || IsInDesignMode)
				return;
			++fpsFrameCounter;
			renderTimer.Tick();
			if (renderTimer.TotalTime - lastFpsUpdate > TimeSpan.FromSeconds(1.0))
			{
				FPS = fpsFrameCounter / (renderTimer.TotalTime - lastFpsUpdate).TotalSeconds;
				lastFpsUpdate = renderTimer.TotalTime;
				fpsFrameCounter = 0;
			}
			Renderer.Render(GetDrawEventArgs());
			InvalidateVisual();
		}

		private void UpdateReallyLoopRendering()
		{
			var newValue = !IsInDesignMode
				&& IsLoopRendering
				&& Renderer != null
				&& surface != null
				&& VisualParent != null
				&& IsVisible;

			newValue = false;

			if (newValue != isReallyLoopRendering)
			{
				isReallyLoopRendering = newValue;
				if (isReallyLoopRendering)
				{
					renderTimer.Resume();
					renderTimer.Tick();
					lastFpsUpdate = renderTimer.TotalTime;
					fpsFrameCounter = 0;
					CompositionTarget.Rendering += OnLoopRendering;
				}
				else
				{
					CompositionTarget.Rendering -= OnLoopRendering;
					renderTimer.Pause();
				}
			}

		}

		private void UpdateSize()
		{
			//Debug.WriteLine("UpdateSize");
			var presentationSource = PresentationSource.FromVisual(this);
			if (Renderer == null || presentationSource == null)
				return;
			var dpiX = 96.0 * presentationSource.CompositionTarget.TransformToDevice.M11;
			var dpiY = 96.0 * presentationSource.CompositionTarget.TransformToDevice.M22;
			var size = DesiredSize;
			surface = new WriteableBitmap((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height), dpiX, dpiY, PixelFormats.Bgra32, null);
			Renderer.Reset(GetDrawEventArgs());
			UpdateReallyLoopRendering();
		}

		#endregion

	}

}
