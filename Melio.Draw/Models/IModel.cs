using SharpDX.Direct3D11;
using Melio.Draw.SharpDX;

namespace Melio.Draw.Models
{
	interface IModel
	{
		void Render(DeviceContext deviceContext, OrthogonalCamera camera);
	}
}
