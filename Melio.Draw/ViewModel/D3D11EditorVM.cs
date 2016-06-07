using SharpDX.Direct3D11;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using Melio.Draw.SharpDX;
using Melio.Draw.Visible;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Melio.Draw.ViewModel
{
	public class D3D11EditorVM : D3D11
	{
		#region private member

		private Background _background;
		private Neatline _neatline;
		private ProjectMV _project;

		#endregion
		#region Constructor
		public D3D11EditorVM(ProjectMV project)
		{
			var device = Device;
			_project = project;

			_background = new Background(device);
			_neatline = new Neatline(device);

			AlphaBlendingEnabled = true;
			ZBufferEnabled = false;

		}
		#endregion
		#region overridden members
		protected override void Dispose(bool disposeManagedResources)
		{
			if (disposeManagedResources)
			{
			}
			base.Dispose(disposeManagedResources);
		}

		protected override void RenderScene(DrawEventArgs args)
		{
			var deviceContext = Device.ImmediateContext;

			deviceContext.VertexShader.SetSampler(0, LinearSamplerState);
			deviceContext.PixelShader.SetSampler(0, LinearSamplerState);


			_background.Render(deviceContext, Camera);
			_project.Render(deviceContext, Camera);
			_neatline.Render(deviceContext, _project);
        }
		#endregion
	}
}