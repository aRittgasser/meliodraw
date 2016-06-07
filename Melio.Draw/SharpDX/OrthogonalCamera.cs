using System.Windows.Input;
using SharpDX;

using Point = System.Windows.Point;
using System.Windows;
using System;
using Melio.Draw.ViewModel;
using System.IO;

namespace Melio.Draw.SharpDX
{
	public class OrthogonalCamera : Component
	{
		// Projection
		private Matrix _projection;
		private float _nearPlane;
		private float _farPlane;
		private float _width;
		private float _height;
		private float _oldWidth;
		private float _oldHeight;

		private Cursor _moveCursor;
		private Cursor _zoomCursor;

		private const float _maxZoom = 1.0e3f;
		private const float _minZoom = 1.0e-3f;
		// View
		private Matrix _view;
		private Vector2 _translation;
		private float _zoom;
		// Camera Movement
		private Point _lastCurPos, _downCurPos;

		public float NearPlane { get { return _nearPlane; } set { _nearPlane = value; updateProjection(); OnPropertyChanged("NearPlane"); } }
		public float FarPlane { get { return _farPlane; } set { _farPlane = value; updateProjection(); OnPropertyChanged("FarPlane"); } }
		public float Width { get { return _width; } set { _width = value; updateProjection(); OnPropertyChanged("Width"); } }
		public float Height { get { return _height; } set { _height = value; updateProjection(); OnPropertyChanged("Height"); } }

		public float Zoom {
			get { return _zoom; }
		}
		

		public Matrix View { get { return _view; } }
		public Matrix Projection { get { return _projection; } }

		public Vector2 Translation { get { return _translation; } }


		#region ctor

		public OrthogonalCamera() : base("OrthogonalCamera") {
			_nearPlane = 0.125f;
			_farPlane = 1024.0f;
			_view = Matrix.Identity;
			_translation = Vector2.Zero;
			_zoom = 1.0f;

			using (Stream stream = Application.GetResourceStream(new Uri("pack://application:,,,/Content/Cursors/zoom.cur ")).Stream)
			{
				_zoomCursor = new Cursor(stream);
			}
			using (Stream stream = Application.GetResourceStream(new Uri("pack://application:,,,/Content/Cursors/move.cur ")).Stream)
			{
				_moveCursor = new Cursor(stream);
			}
		}

		#endregion

		public void SetProject(ProjectMV project)
		{
			_translation = new Vector2(-project.Width/2.0f, -project.Height/2.0f);
			_zoom = 1.0f;
			OnResize();
		}

		public void OnMouseDown(MouseEventArgs param, ref bool isScreenDirty)
		{
			var sender = param.Source as IInputElement;
			if (sender == null)
				return;
			_lastCurPos = param.GetPosition(sender);
			_downCurPos = _lastCurPos;
        }

		public void OnMouseUp(MouseEventArgs param, ref bool isScreenDirty)
		{
			//Mouse.OverrideCursor = Cursors.Arrow;
        }

		public void OnMouseMove(MouseEventArgs param, ref bool isScreenDirty)
		{
			var sender = param.Source as IInputElement;
			if (sender == null)
				return;
			var curPos = param.GetPosition(sender);
			// Translate
			if (param.MiddleButton == MouseButtonState.Pressed)
			{
				_translation += UnProject(curPos - _lastCurPos);
				
                OnResize();
				Mouse.OverrideCursor = _moveCursor;
				//Mouse.OverrideCursor = Cursors.ScrollAll;
				//sender.Render();
				isScreenDirty = true;
			}
			// Zoom
			else if(param.RightButton == MouseButtonState.Pressed)
			{
				Vector2 center = UnProject(_downCurPos);
				double delta = (curPos - _lastCurPos).X + (curPos - _lastCurPos).Y;
				float oldZoom = _zoom;
                _zoom *= (float)Math.Pow(1.1, delta / 20.0);
				_zoom = Math.Min(_zoom, _maxZoom);
				_zoom = Math.Max(_zoom, _minZoom);
				_translation += center * (oldZoom-_zoom);

				OnResize();
				OnPropertyChanged("Zoom");
				Mouse.OverrideCursor = _zoomCursor;
				isScreenDirty = true;
				//sender.Render();
			}
			_lastCurPos = curPos;
		}

		public void OnMouseWheel(MouseWheelEventArgs param, ref bool isScreenDirty)
		{
			var sender = param.Source as IInputElement;
			if (sender == null)
				return;
			var curPos = param.GetPosition(sender);
			Vector2 center = UnProject(curPos);
			double delta = param.Delta;
            float oldZoom = _zoom;
			_zoom *= (float)Math.Pow(1.1, delta / 60.0);
			_zoom = Math.Min(_zoom, _maxZoom);
			_zoom = Math.Max(_zoom, _minZoom);
			OnPropertyChanged("Zoom");

			_translation += center * (oldZoom - _zoom);

			param.Handled = true;

			OnResize();
			//sender.Render();
			isScreenDirty = true;
		}

		public void OnMovementEnded()
		{

		}

		public void OnResize()
		{
			_view.M11 = _zoom;
			_view.M22 = _zoom;
			_translation.X += (_width - _oldWidth)/2.0f;
			_translation.Y += (_height - _oldHeight)/2.0f;
			_view.M41 = _translation.X - _width / 2.0f;
			_view.M42 = _translation.Y - _height / 2.0f;

			_oldWidth = _width;
			_oldHeight = _height;
        }

		public Vector2 Project(Vector2 p)
		{
			return new Vector2(p.X * _zoom + _translation.X, _height-_translation.Y -(p.Y * _zoom) );
		}

		public float ProjectX(float x)
		{
			return x * _zoom + _translation.X;
		}

		public float ProjectY(float y)
		{
			return _height - _translation.Y - (y * _zoom);
		}

		public Vector2 UnProject(Point p)
		{
			return (new Vector2((float)p.X, _height - (float)p.Y) - _translation)/_zoom;
		}

		public Vector2 UnProject(Vector2 p)
		{
			return (new Vector2(p.X, _height - p.Y) - _translation) / _zoom;
		}

		public Vector2 UnProject(Vector p)
		{
			return new Vector2((float)p.X, -(float)p.Y);
		}

		private void updateProjection()
		{
			Matrix.OrthoOffCenterLH(-_width/2.0f, _width / 2.0f, -_height/2.0f, _height / 2.0f, _nearPlane, _farPlane, out _projection);
			OnResize();
        }

	}
}
