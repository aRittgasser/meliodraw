using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Melio.Draw.SharpDX;
using SharpDX.Direct3D11;
using SharpDX;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Melio.Draw.ViewModel;

namespace Melio.Draw.Tools
{
	public class Selection : BaseTool
	{
		private enum Section
		{
			None, Top, TopRight, Right, BottomRight,
			Bottom, BottomLeft, Left, TopLeft, Move
		}

		public override Uri ImageSourceUri { get { return new Uri("pack://application:,,,/Content/Tools/selecter.png", UriKind.Absolute); } }
		public override string Tooltip { get { return "Auswahl Werkzeug"; } }

		//private ObservableCollection<Models.BaseModel> _items;
		private SelectionManager _items;
		private OrthogonalCamera _camera;

		public float MinMoveDistance = 4.0f; // Insert into Options!

		public SelectionManager Items { get { return _items; } }

		private List<Cursor> _cursors;

		private Section _state = Section.None;
		private Vector2 _mouseDownPos;

		private Vector2 _selectedMouseDownPosition;

		public Selection(SelectionManager items, OrthogonalCamera camera)
		{
			_items = items;
			_camera = camera;

			_cursors = new List<Cursor>();
			for (int i = 0; i < 8; i++)
			{
				using (Stream stream = Application.GetResourceStream(new Uri(string.Format("pack://application:,,,/Content/Cursors/scale/{0}.cur", i))).Stream)
				{
					_cursors.Add(new Cursor(stream));
				}
			}
		}

		public override bool OnMouseDown(MouseEventArgs param, ref bool isScreenDirty)
		{
			var sender = param.Source as DXElement;
			if (sender == null)
				return false;
			_mouseDownPos = _camera.UnProject(param.GetPosition(sender));

			if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
			{
				foreach (var item in _items.AllItems)
				{
					if (item.Hit(_mouseDownPos))
					{
						isScreenDirty = true;
						_items.ToggleSelection(item);
					}
				}
				return isScreenDirty;
			}
			else
			{
				switch (_state)
				{
					case Section.None:
						foreach (var item in _items.AllItems)
						{
							if (item.Hit(_mouseDownPos))
							{
								isScreenDirty = _items.SetSelection(item);
								_selectedMouseDownPosition = _items.FocusedItem.Position;
								_state = Section.Move;
								return isScreenDirty;
							}
						}
						isScreenDirty = _items.DeSelectAll();
						return isScreenDirty;
					case Section.Move:
						//foreach (var item in _items.SelectedItems)
						//{
						//	if (item.Hit(_mouseDownPos))
						//	{
						//		_bodyDownPosition = item.Position;
						//                          return false;
						//	}
						//}

						//foreach (var item in _items.AllItems)
						//{
						//	if (item.Hit(_mouseDownPos))
						//	{
						//		isScreenDirty = _items.SetSelection(item);
						//		_selectedMouseDownPosition = _items.FocusedItem.Position;
						//		return isScreenDirty;
						//	}
						//}

						if (_items.FocusedItem.Hit(_mouseDownPos))
						{
							_selectedMouseDownPosition = _items.FocusedItem.Position;
							return false;
						}
						return false;
					default:
						if(_items.FocusedItem != null)
							_selectedMouseDownPosition = _items.FocusedItem.Position;
						return false;
				}
			}
		}

		public override bool OnMouseMove(MouseEventArgs param, ref bool isScreenDirty)
		{
			var sender = param.Source as DXElement;
			if (sender == null)
				return false;
			Vector2 curPos = _camera.UnProject(param.GetPosition(sender));
			// Look if the cursor is near the Nodes

			if (param.LeftButton == MouseButtonState.Pressed)
			{
				var selected = _items.FocusedItem;

				if (selected == null)
					return false;


				float dX, dY;
				Vector2 s = selected.Scale;
				Vector2 p = _selectedMouseDownPosition;
				switch (_state)
				{
					case Section.Move:
						if ((curPos - _mouseDownPos).LengthSquared() > MinMoveDistance)
							selected.Position = _selectedMouseDownPosition + (curPos - _mouseDownPos);
						else
							selected.Position = _selectedMouseDownPosition;
						isScreenDirty = true;
						if (Mouse.OverrideCursor == null)
							Mouse.OverrideCursor = Cursors.SizeAll;
						break;
					case Section.Right:
						dX = curPos.X - _mouseDownPos.X;
						s.X = 1.0f + dX / selected.Size.X;
						selected.Scale = s;
						p.X += dX /  4.0f;
						//selected.Position = p- _selectedMouseDownPosition;
						isScreenDirty = true;
						break;
					case Section.Top:
						dY = (curPos - _mouseDownPos).Y;
						s.Y = 1.0f + dY / selected.Size.Y;
						selected.Scale = s;
						isScreenDirty = true;
						break;
					case Section.TopRight:
						dY = curPos.Y - _mouseDownPos.Y;
						dX = curPos.X - _mouseDownPos.X;
						s.X = 1.0f + dX / selected.Size.X;
						s.Y = 1.0f + dY / selected.Size.Y;
						selected.Scale = s;
						isScreenDirty = true;
						break;
					default:
						break;
				}
			}
			else
			{
				_state = getCurrentSelection(curPos);

				switch (_state)
				{
					case Section.Top:
					case Section.Bottom:
						Mouse.OverrideCursor = Cursors.SizeNS;
						break;
					case Section.Left:
					case Section.Right:
						Mouse.OverrideCursor = Cursors.SizeWE;
						break;
					case Section.BottomLeft:
					case Section.TopRight:
						Mouse.OverrideCursor = Cursors.SizeNESW;
						break;
					case Section.BottomRight:
					case Section.TopLeft:
						Mouse.OverrideCursor = Cursors.SizeNWSE;
						break;
					case Section.Move:
						Mouse.OverrideCursor = Cursors.SizeAll;
						break;
					default:
						Mouse.OverrideCursor = null;
						break;
				}
			}

			return false;
		}

		public override bool OnMouseUp(MouseEventArgs param, ref bool isScreenDirty)
		{
			return false;
		}

		public override void Render(DeviceContext deviceContext, OrthogonalCamera camera)
		{
		}

		private Section getCurrentSelection(Vector2 curPos)
		{
			var selected = _items.FocusedItem;
			if (selected == null)
				return Section.None;

			// Look if the cursor is near the Nodes
			Vector2 bl = selected.BottomLeft;
			Vector2 br = selected.BottomRight;
			Vector2 tl = selected.TopLeft;
			Vector2 tr = selected.TopRight;

			Vector2 bottom = selected.Bottom;
			Vector2 top = selected.Top;
			Vector2 left = selected.Left;
			Vector2 right = selected.Right;

			const float dist = 40.0f;

			if ((top - curPos).LengthSquared() < dist)
				return Section.Top;
			if ((bottom - curPos).LengthSquared() < dist)
				return Section.Bottom;
			if ((left - curPos).LengthSquared() < dist)
				return Section.Left;
			if ((right - curPos).LengthSquared() < dist)
				return Section.Right;
			if ((bl - curPos).LengthSquared() < dist)
				return Section.BottomLeft;
			if ((br - curPos).LengthSquared() < dist)
				return Section.BottomRight;
			if ((tl - curPos).LengthSquared() < dist)
				return Section.TopLeft;
			if ((tr - curPos).LengthSquared() < dist)
				return Section.TopRight;
			if (selected.Hit(curPos))
				return Section.Move;

			return Section.None;
		}
	}
}
