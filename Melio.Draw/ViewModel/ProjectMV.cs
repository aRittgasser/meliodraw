using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using Melio.Draw.SharpDX;
using SharpDX;
using SharpDX.Direct3D11;

using Point = System.Windows.Point;
using System.Collections.ObjectModel;

namespace Melio.Draw.ViewModel
{
	public class ProjectMV : DisplayNameBaseVM, IDisposable
	{
		#region Properties
		public static readonly DependencyProperty D3D11Property;
		private static readonly DependencyPropertyKey D3D11PropertyKey;

		public static readonly DependencyProperty WidthProperty;
		private static readonly DependencyPropertyKey WidthPropertyKey;
		public static readonly DependencyProperty HeightProperty;
		private static readonly DependencyPropertyKey HeightPropertyKey;

		public static readonly DependencyProperty ToolsProperty;
		public static readonly DependencyProperty SelectedToolProperty;
		//public static readonly DependencyProperty ElementsProperty;

		public static readonly DependencyProperty ItemsProperty;


		public static readonly DependencyProperty MouseDownCommandProperty;
		private static readonly DependencyPropertyKey MouseDownCommandPropertyKey;
		public static readonly DependencyProperty MouseMoveCommandProperty;
		private static readonly DependencyPropertyKey MouseMoveCommandPropertyKey;
		public static readonly DependencyProperty MouseUpCommandProperty;
		private static readonly DependencyPropertyKey MouseUpCommandPropertyKey;
		public static readonly DependencyProperty MouseWheelCommandProperty;
		private static readonly DependencyPropertyKey MouseWheelCommandPropertyKey;
		public static readonly DependencyProperty KeyDownCommandProperty;
		private static readonly DependencyPropertyKey KeyDownCommandPropertyKey;
		public static readonly DependencyProperty KeyUpCommandProperty;
		private static readonly DependencyPropertyKey KeyUpCommandPropertyKey;

		public static readonly DependencyProperty MouseEnterCommandProperty;
		private static readonly DependencyPropertyKey MouseEnterCommandPropertyKey;

		public static readonly DependencyProperty MouseLeaveCommandProperty;
		private static readonly DependencyPropertyKey MouseLeaveCommandPropertyKey;

		public static readonly DependencyProperty PreviewMouseWheelCommandProperty;
		private static readonly DependencyPropertyKey PreviewMouseWheelCommandPropertyKey;

		public D3D11 D3D11 { get { return (D3D11)GetValue(D3D11Property); } set { SetValue(D3D11PropertyKey, value); } }
		/// <summary>
		/// Gets or sets the width of the working space.
		/// </summary>
		/// <value>
		/// The width of the working space.
		/// </value>
		public float Width { get { return (float)GetValue(WidthProperty); } set { SetValue(WidthPropertyKey, value); } }
		/// <summary>
		/// Gets or sets the height of the working space.
		/// </summary>
		/// <value>
		/// The height of the working space.
		/// </value>
		public float Height { get { return (float)GetValue(HeightProperty); } set { SetValue(HeightPropertyKey, value); } }
		/// <summary>
		/// Gets the camera.
		/// </summary>
		/// <value>
		/// The camera.
		/// </value>
		public OrthogonalCamera Camera { get { return _camera; } }

		public ObservableCollection<Tools.BaseTool> Tools { get { return GetValue(ToolsProperty) as ObservableCollection<Tools.BaseTool>; } set { SetValue(ToolsProperty, value); } }
		public Tools.BaseTool SelectedTool { get { return GetValue(SelectedToolProperty) as Tools.BaseTool; } set { SetValue(SelectedToolProperty, value); } }
		//public ObservableCollection<Models.BaseModel> Elements { get { return GetValue(ElementsProperty) as ObservableCollection<Models.BaseModel>; } set { SetValue(ElementsProperty, value); } }

		public SelectionManager Items { get { return GetValue(ItemsProperty) as SelectionManager; } set{ SetValue(ItemsProperty, value); } }

		public Point CurserPos { get { return _curserPos; } }

		public ICommand MouseDownCommand { get { return (ICommand)GetValue(MouseDownCommandProperty); } private set { SetValue(MouseDownCommandPropertyKey, value); } }
		public ICommand MouseMoveCommand { get { return (ICommand)GetValue(MouseMoveCommandProperty); } private set { SetValue(MouseMoveCommandPropertyKey, value); } }
		public ICommand MouseUpCommand { get { return (ICommand)GetValue(MouseUpCommandProperty); } private set { SetValue(MouseUpCommandPropertyKey, value); } }
		public ICommand MouseWheelCommand { get { return (ICommand)GetValue(MouseWheelCommandProperty); } private set { SetValue(MouseWheelCommandPropertyKey, value); } }
		public ICommand KeyDownCommand { get { return (ICommand)GetValue(KeyDownCommandProperty); } private set { SetValue(KeyDownCommandPropertyKey, value); } }
		public ICommand KeyUpCommand { get { return (ICommand)GetValue(KeyUpCommandProperty); } private set { SetValue(KeyUpCommandPropertyKey, value); } }

		public ICommand MouseEnterCommand { get { return (ICommand)GetValue(MouseEnterCommandProperty); } private set { SetValue(MouseEnterCommandPropertyKey, value); } }
		public ICommand MouseLeaveCommand { get { return (ICommand)GetValue(MouseLeaveCommandProperty); } private set { SetValue(MouseLeaveCommandPropertyKey, value); } }


		public ICommand PreviewMouseWheelCommand { get { return (ICommand)GetValue(PreviewMouseWheelCommandProperty); } private set { SetValue(PreviewMouseWheelCommandPropertyKey, value); } }

		#endregion
		#region Constructor and Destructor
		static ProjectMV(){
			D3D11PropertyKey = DependencyProperty.RegisterReadOnly("D3D11", typeof(D3D11), typeof(ProjectMV), new PropertyMetadata(null));
			D3D11Property = D3D11PropertyKey.DependencyProperty;

			WidthPropertyKey = DependencyProperty.RegisterReadOnly("Width", typeof(float), typeof(ProjectMV), new PropertyMetadata(null));
			WidthProperty = WidthPropertyKey.DependencyProperty;
			HeightPropertyKey = DependencyProperty.RegisterReadOnly("Height", typeof(float), typeof(ProjectMV), new PropertyMetadata(null));
			HeightProperty = HeightPropertyKey.DependencyProperty;

			ToolsProperty = DependencyProperty.Register("Tools", typeof(ObservableCollection<Tools.BaseTool>), typeof(ProjectMV));
			SelectedToolProperty = DependencyProperty.Register("SelectedTool", typeof(Tools.BaseTool), typeof(ProjectMV));
			//ElementsProperty = DependencyProperty.Register("Elements", typeof(ObservableCollection<Models.BaseModel>), typeof(ProjectMV));
			ItemsProperty = DependencyProperty.Register("Items", typeof(SelectionManager), typeof(ProjectMV));

			MouseDownCommandPropertyKey = DependencyProperty.RegisterReadOnly("MouseDownCommand", typeof(ICommand), typeof(ProjectMV), new PropertyMetadata(null));
			MouseDownCommandProperty = MouseDownCommandPropertyKey.DependencyProperty;
			MouseMoveCommandPropertyKey = DependencyProperty.RegisterReadOnly("MouseMoveCommand", typeof(ICommand), typeof(ProjectMV), new PropertyMetadata(null));
			MouseMoveCommandProperty = MouseMoveCommandPropertyKey.DependencyProperty;
			MouseUpCommandPropertyKey = DependencyProperty.RegisterReadOnly("MouseUpCommand", typeof(ICommand), typeof(ProjectMV), new PropertyMetadata(null));
			MouseUpCommandProperty = MouseUpCommandPropertyKey.DependencyProperty;
			MouseWheelCommandPropertyKey = DependencyProperty.RegisterReadOnly("MouseWheelCommand", typeof(ICommand), typeof(ProjectMV), new PropertyMetadata(null));
			MouseWheelCommandProperty = MouseWheelCommandPropertyKey.DependencyProperty;
			KeyDownCommandPropertyKey = DependencyProperty.RegisterReadOnly("KeyDownCommand", typeof(ICommand), typeof(ProjectMV), new PropertyMetadata(null));
			KeyDownCommandProperty = KeyDownCommandPropertyKey.DependencyProperty;
			KeyUpCommandPropertyKey = DependencyProperty.RegisterReadOnly("KeyUpCommand", typeof(ICommand), typeof(ProjectMV), new PropertyMetadata(null));
			KeyUpCommandProperty = KeyUpCommandPropertyKey.DependencyProperty;

			MouseEnterCommandPropertyKey = DependencyProperty.RegisterReadOnly("MouseEnterCommand", typeof(ICommand), typeof(ProjectMV), new PropertyMetadata(null));
			MouseEnterCommandProperty = MouseEnterCommandPropertyKey.DependencyProperty;
			MouseLeaveCommandPropertyKey = DependencyProperty.RegisterReadOnly("MouseLeaveCommand", typeof(ICommand), typeof(ProjectMV), new PropertyMetadata(null));
			MouseLeaveCommandProperty = MouseLeaveCommandPropertyKey.DependencyProperty;

			PreviewMouseWheelCommandPropertyKey = DependencyProperty.RegisterReadOnly("PreviewMouseWheelCommand", typeof(ICommand), typeof(ProjectMV), new PropertyMetadata(null));
			PreviewMouseWheelCommandProperty = PreviewMouseWheelCommandPropertyKey.DependencyProperty;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectMV"/> class.
		/// Only use this version in DesignMode!
		/// </summary>
		/// <exception cref="PlatformNotSupportedException"></exception>
		public ProjectMV() : base("Unbenanntes Project")
		{
			if (!IsInDesignMode)
				throw new PlatformNotSupportedException();
		}

		public ProjectMV(MainWindowVM mainWindow, DXElement dxElement) : base("Unbenanntes Project")
		{
			isDisposed = false;
			_mainWindow = mainWindow;
			_dxElement = dxElement;

			MouseDownCommand = new RelayCommand(p => handleMouseDown(p as MouseEventArgs));
			MouseMoveCommand = new RelayCommand(p => handleMouseMove(p as MouseEventArgs), p => canHandleMouseMove(p as MouseEventArgs));
			MouseUpCommand = new RelayCommand(p => handleMouseUp(p as MouseEventArgs));
			MouseWheelCommand = new RelayCommand(p => handleMouseWheel(p as MouseWheelEventArgs));
			PreviewMouseWheelCommand = new RelayCommand(p => handlePreviewMouseWheel(p as MouseWheelEventArgs));
			KeyDownCommand = new RelayCommand(p => handleKeyDown(p as KeyEventArgs));
			KeyUpCommand = new RelayCommand(p => handleKeyUp(p as KeyEventArgs));

			MouseEnterCommand = new RelayCommand(p => handleMouseEnter(p as MouseEventArgs));
			MouseLeaveCommand = new RelayCommand(p => handleMouseLeave(p as MouseEventArgs));


			//_models = new List<Models.BaseModel>();
			//Elements = new ObservableCollection<Models.BaseModel>();
			Items = new SelectionManager();
			_camera = new OrthogonalCamera();
            D3D11 = new D3D11EditorVM(this) { Camera = _camera};
			//CurrentTool = new Tools.Rectangle(D3D11, _models, _camera);
			Tools = new ObservableCollection<Tools.BaseTool>();
			Tools.Add(new Tools.Rectangle(D3D11, Items, _camera));
			Tools.Add(new Tools.Selection(Items, _camera));
			SelectedTool = Tools[0];
			// Initialize the GPU Data
			Models.Polygon.Init(D3D11);
			Models.BaseModel.Init(D3D11, dxElement);
		}
		~ProjectMV()
		{
			Dispose(false);
		}
		#endregion
		#region private fields
		private bool isDisposed;
		private MainWindowVM _mainWindow;
		private DXElement _dxElement;

		private Point _curserPos;

		//private Tools.BaseTool _tool;
		//private List<Models.BaseModel> _models;
		private OrthogonalCamera _camera;
		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing)
				{
					if (D3D11 != null)
					{
						D3D11.Dispose();
						D3D11 = null;
					}
				}
				isDisposed = true;
			}
		}
		#region public functions
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public static ProjectMV CreateNewProject(MainWindowVM mainWindow, DXElement dxElement)
		{
			var project = new ProjectMV(mainWindow, dxElement);
			project.Width = 320.0f;
			project.Height = 240.0f;
			project.Camera.SetProject(project);
            return project;
        }

		public void Render(DeviceContext deviceContext, OrthogonalCamera camera)
		{
			Items.Render(deviceContext, camera);
			SelectedTool.Render(deviceContext, camera);
		}

		#endregion
		#region Event Handler
		private void handleMouseDown(MouseEventArgs param)
		{
			_dxElement.Focusable = true;
			Keyboard.Focus(_dxElement);
			if (param == null)
				throw new InvalidOperationException(GetFunctionName());

			bool isScreenDirty = false;

			if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
				_camera.OnMouseDown(param, ref isScreenDirty);
			else
				SelectedTool.OnMouseDown(param, ref isScreenDirty);
			//_tool.OnMouseDown(param);
			if(isScreenDirty)
			{
				// Render
				_dxElement.Render();
            }
		}

		private bool canHandleMouseMove(MouseEventArgs param)
		{
			
            return true;
		}

		private void handleMouseMove(MouseEventArgs param)
		{
			if (param == null)
				throw new InvalidOperationException(GetFunctionName());
			var sender = param.Source as IInputElement;
			if (sender == null)
				throw new InvalidOperationException(GetFunctionName());

			//var dx = sender as DXElement;

			_curserPos = Mouse.GetPosition(sender);
			bool isScreenDirty = false;

			// Wurde die Kamera Bewegung aktiviert?
			if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
			{
				_camera.OnMouseMove(param, ref isScreenDirty);
			}
			else
			{
				// Welches Werkzeug ist ausgewählt? 
				SelectedTool.OnMouseMove(param, ref isScreenDirty);
				//_tool.OnMouseMove(param);
			}
			if (isScreenDirty)
				_dxElement.Render();

			//System.Diagnostics.Debug.WriteLine(param);
		}

		private void handleMouseUp(MouseEventArgs param)
		{
			if (param == null)
				throw new InvalidOperationException(GetFunctionName());
			var sender = param.Source as IInputElement;
			if (sender == null)
				throw new InvalidOperationException(GetFunctionName());

			bool isScreenDirty = false;

			// Ist die Kamera Bewegung aktiviert?
			if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
			{
				_camera.OnMouseUp(param, ref isScreenDirty);
			}
			else
			{
				SelectedTool.OnMouseUp(param, ref isScreenDirty);
				//_tool.OnMouseUp(param);
			}
			Mouse.OverrideCursor = null;
			if (isScreenDirty)
				_dxElement.Render();
		}

		private void handleMouseWheel(MouseWheelEventArgs param)
		{
			bool isScreenDirty = false;
			_camera.OnMouseWheel(param, ref isScreenDirty);
			if (isScreenDirty)
				_dxElement.Render();
		}

		private void handleMouseEnter(MouseEventArgs param)
		{
			//throw new NotImplementedException();
		}

		private void handleMouseLeave(MouseEventArgs param)
		{
			//throw new NotImplementedException();
		}

		private void handlePreviewMouseWheel(MouseWheelEventArgs param)
		{
		}

		private void handleKeyDown(KeyEventArgs param)
		{
			bool isScreenDirty = false;
			System.Diagnostics.Debug.WriteLine(param);
			if (param.Key == Key.Delete)
			{
				isScreenDirty = Items.DeleteSelected();
                param.Handled = true;
            }
			else if (param.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control)>0)
			{
				isScreenDirty = Items.SelectAll();
                param.Handled = true;
			}
			else if (param.Key == Key.Q && Keyboard.Modifiers == ModifierKeys.None)
			{
				SelectedTool = Tools[1];
			}
			else if (param.Key == Key.R && Keyboard.Modifiers == ModifierKeys.None)
			{
				SelectedTool = Tools[0];
			}

			if (isScreenDirty)
				_dxElement.Render();
		}

		private void handleKeyUp(KeyEventArgs param)
		{
			bool isScreenDirty = false;

			if (isScreenDirty)
				_dxElement.Render();
		}
		#endregion
	}
}
