using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharpDX;
using SharpDX.Direct3D11;
using Melio.Draw.SharpDX;
using Component = SharpDX.Component;
using System;

namespace Melio.Draw.Tools
{
	public abstract class BaseTool : Component, INotifyPropertyChanged
	{
		public new event PropertyChangedEventHandler PropertyChanged;

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public abstract Uri ImageSourceUri { get; }
		public abstract string Tooltip { get;}

		public abstract void Render(DeviceContext deviceContext, OrthogonalCamera camera);
		public abstract bool OnMouseMove(MouseEventArgs param, ref bool isScreenDirty);
		public abstract bool OnMouseUp(MouseEventArgs param, ref bool isScreenDirty);
		public abstract bool OnMouseDown(MouseEventArgs param, ref bool isScreenDirty);
	}

}
