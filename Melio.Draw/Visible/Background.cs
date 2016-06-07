using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using Melio.Draw.SharpDX;
using Melio.Draw.Extensions;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Melio.Draw.Visible
{
	public class Background : Component
	{
		#region private member
#if DEBUG
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness | ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#else
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness;
#endif

		private Buffer _globalBuffer;

		private VertexShader _vertexShader;
		private PixelShader _pixelShader;
		private InputLayout _inputLayout;

		private Buffer _buffer;
		private Buffer _indexBuffer;
		private VertexBufferBinding _bufferBinding;

		Vector4 _color1, _color2;
		Vector4 _tileSize;
		#endregion

		public Background(Device device)
		{
			_color1 = new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
			_color2 = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
			_tileSize = new Vector4(15.0f, 15.0f, 0.0f, 0.0f);
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Background.hlsl", "VS", "vs_5_0"))
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Visible\Background.hlsl", "VS", "vs_5_0", shaderFlags, EffectFlags.None))
			{
				_vertexShader = ToDispose(new VertexShader(device, bytecode));
				_inputLayout = ToDispose(new InputLayout(device, bytecode, new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0) }));
			}
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Background.hlsl", "PS", "ps_5_0"))
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Visible\Background.hlsl", "PS", "ps_5_0", shaderFlags, EffectFlags.None))
			{
				_pixelShader = ToDispose(new PixelShader(device, bytecode));
			}

			_globalBuffer = ToDispose(new Buffer(device, Matrix.SizeInBytes + 2 * Vector4.SizeInBytes + Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

			using (var dataStream = new DataStream(Vector3.SizeInBytes * 4, true, true))
			{
				dataStream.Write(new Vector3(-0.5f, -0.5f, 1.0f));
				dataStream.Write(new Vector3(-0.5f, 0.5f, 1.0f));
				dataStream.Write(new Vector3(0.5f, 0.5f, 1.0f));
				dataStream.Write(new Vector3(0.5f, -0.5f, 1.0f));

				dataStream.Position = 0;
				_buffer = ToDispose(new Buffer(device, dataStream, Vector3.SizeInBytes * 4, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				_bufferBinding = new VertexBufferBinding(_buffer, Vector3.SizeInBytes, 0);
			}
			using (var dataStream = new DataStream(sizeof(int) * 6, true, true))
			{
				dataStream.Write(0);
				dataStream.Write(1);
				dataStream.Write(2);
				dataStream.Write(2);
				dataStream.Write(3);
				dataStream.Write(0);
				dataStream.Position = 0;
				_indexBuffer = ToDispose(new Buffer(device, dataStream, sizeof(int) * 6, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
			}
		}

		public void Render(DeviceContext deviceContext, OrthogonalCamera camera)
		{
			Matrix projection = camera.Projection;
			_tileSize.Z = -camera.Width / 2.0f;
			_tileSize.W = -camera.Height / 2.0f;
			Matrix backgroundWorld = Matrix.Scaling(camera.Width, camera.Height, 1.0f);

			Matrix worldViewProjection;

			//Matrix.Multiply(ref backgroundWorld, ref view, out worldViewProjection);
			Matrix.Multiply(ref backgroundWorld, ref projection, out worldViewProjection);
			// Transpose local Matrices
			Matrix.Transpose(ref worldViewProjection, out worldViewProjection);

			deviceContext.VertexShader.SetConstantBuffers(0, _globalBuffer);
			deviceContext.PixelShader.SetConstantBuffers(0, _globalBuffer);

			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			deviceContext.InputAssembler.InputLayout = _inputLayout;
			deviceContext.VertexShader.Set(_vertexShader);
			deviceContext.HullShader.Set(null);
			deviceContext.DomainShader.Set(null);
			deviceContext.PixelShader.Set(_pixelShader);

			deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
			deviceContext.InputAssembler.SetVertexBuffers(0, _bufferBinding);

			DataStream dataStream;
			deviceContext.MapSubresource(_globalBuffer, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.Write(worldViewProjection);
			dataStream.Write(_color1);
			dataStream.Write(_color2);
			dataStream.Write(_tileSize);
			deviceContext.UnmapSubresource(_globalBuffer, 0);

			deviceContext.DrawIndexed(6, 0, 0);
		}
	}
}