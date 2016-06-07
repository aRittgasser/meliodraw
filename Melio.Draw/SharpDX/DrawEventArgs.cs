using System;
using System.Windows;
using System.Windows.Media.Imaging;
using SharpDX;

namespace Melio.Draw.SharpDX
{
	public class DrawEventArgs : EventArgs
	{

		#region Eigenschaften

		public TimerTick Timer { get; private set; }
		public Size RenderSize { get; private set; }
		public WriteableBitmap Target { get; private set; }

		#endregion

		#region ctor

		public DrawEventArgs(TimerTick timer, Size renderSize, WriteableBitmap target)
		{
			Timer = timer;
			RenderSize = renderSize;
			Target = target;
		}

		#endregion

	}

}
