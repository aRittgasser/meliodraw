using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.D3DCompiler;
using Melio.Draw.SharpDX;
using Melio.Draw.Models;
using Melio.Draw.Extensions;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Melio.Draw.ViewModel;

namespace Melio.Draw.Tools
{
	class Rectangle : BaseTool
	{
		#region private fields
		private static int _RectangleCounter = 0;
        private Vector2 _downPos, _curPos;
		private bool _isVisible;
		//private ICollection<BaseModel> _models;
		private SelectionManager _items;
		OrthogonalCamera _camera;

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

		//private Vector4 _color;
		private ColorVM _color;
		public ColorVM Color
		{
			get { return _color; }
			set
			{
				if (value != _color)
				{
					_color = value;
					OnPropertyChanged();
                }
			}
		}

		public override Uri ImageSourceUri { get { return new Uri("pack://application:,,,/Content/Tools/rectangle.png", UriKind.Absolute); } }
		public override string Tooltip { get { return "Rechteck"; } }

		#endregion
		#region Constructor		
		/// <summary>
		/// Initializes a new instance of the <see cref="Rectangle"/> class.
		/// Only use this Xaml
		/// </summary>
		public Rectangle()
		{
			_color = new ColorVM(0.9f, 0.4f, 0.0f, 0.4f);
		}

		public Rectangle(D3D11 d3d, SelectionManager items, OrthogonalCamera camera)
		{
			//_models = items;
			_items = items;
			_camera = camera;
            Device device = d3d.Device;
			//_color = new Vector4(0.0f, 0.0f, 0.0f, 0.1f);
			_color = new ColorVM(0.9f, 0.4f, 0.0f, 0.4f);
			//_color = new Vector4(0.9f, 0.4f, 0.0f, 0.4f);
			_isVisible = false;

			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Tools.Rectangle.hlsl", "VS", "vs_5_0"))
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Tools.Rectangle.hlsl", "VS", "vs_5_0", shaderFlags, EffectFlags.None))
			{
				_vertexShader = ToDispose(new VertexShader(device, bytecode));
				_inputLayout = ToDispose(new InputLayout(device, bytecode, new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0) }));
            }
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Tools.Rectangle.hlsl", "PS", "ps_5_0"))
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Tools.Rectangle.hlsl", "PS", "ps_5_0", shaderFlags, EffectFlags.None))
			{
				_pixelShader = ToDispose(new PixelShader(device, bytecode));
			}

			_globalBuffer = ToDispose(new Buffer(device, Matrix.SizeInBytes + Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

			using (var dataStream = new DataStream(Vector3.SizeInBytes * 4, true, true))
			{
				dataStream.Write(new Vector3(0.0f, 0.0f, 1.0f));
				dataStream.Write(new Vector3(0.0f, 1.0f, 1.0f));
				dataStream.Write(new Vector3(1.0f, 1.0f, 1.0f));
				dataStream.Write(new Vector3(1.0f, 0.0f, 1.0f));

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
		#endregion
		#region Events
		public override bool OnMouseDown(MouseEventArgs param, ref bool isScreenDirty)
		{
			var sender = param.Source as IInputElement;
			if (sender == null)
				return false;
            _downPos = _camera.UnProject(param.GetPosition(sender));
			return true;
		}

		public override bool OnMouseMove(MouseEventArgs param, ref bool isScreenDirty)
		{
			if (param.LeftButton == MouseButtonState.Pressed)
			{
				var sender = param.Source as DXElement;
				if (sender == null)
					return false;
				_isVisible = true;
				_curPos = _camera.UnProject(param.GetPosition(sender));
				//sender.Render();
				isScreenDirty = true;
				return true;
			}
			else
			{
				if (_isVisible)
				{
					_isVisible = false;
					var sender = param.Source as DXElement;
					if (sender == null)
						return false;
					//sender.Render();
					isScreenDirty = true;
                }
				return false;
			}

		}

		public override bool OnMouseUp(MouseEventArgs param, ref bool isScreenDirty)
		{
			var sender = param.Source as DXElement;
			if (sender == null)
				return false;
			if (_isVisible)
			{
				_isVisible = false;
				// Create new Polygon
				float top = Math.Max(_downPos.Y, _curPos.Y);
				float bottom = Math.Min(_downPos.Y, _curPos.Y);
				float left = Math.Min(_downPos.X, _curPos.X);
				float right = Math.Max(_downPos.X, _curPos.X);
				float centerX = (top + bottom) / 2.0f;
				float centerY = (left + right) / 2.0f;
				left -= centerX;
				right -= centerX;
				top -= centerY;
				bottom -= centerY;
				var vertices = new List<Polygon.Vertex>() {
					new Polygon.Vertex() {Id=0, Y=top,    X=left},
					new Polygon.Vertex() {Id=1, Y=top,    X=right},
					new Polygon.Vertex() {Id=2, Y=bottom, X=right},
					new Polygon.Vertex() {Id=3, Y=bottom, X=left}
				};
				var position = new Vector2(centerX, centerY);
				var polygon = new Polygon(position, vertices, Color,string.Format("Rectangle{0}", _RectangleCounter++));
				//_models.Add(polygon);
				_items.AddItem(polygon);
				isScreenDirty = true;
                sender.Render();
			}
			if (_downPos == default(Vector2))
				return false;
			_downPos = default(Vector2);
			return true;
        }
		#endregion
		public override void Render(DeviceContext deviceContext, OrthogonalCamera camera)
		{
			if (!_isVisible)
				return;
			Matrix view = camera.View;
			Matrix projection = camera.Projection;

			float top = Math.Max(_downPos.Y,_curPos.Y);
			float bottom = Math.Min(_downPos.Y,_curPos.Y);
			float left = Math.Min(_downPos.X,_curPos.X);
			float right = Math.Max(_downPos.X,_curPos.X);

			Matrix world = Matrix.Identity;
			world.M11 = right - left;
			world.M22 = top - bottom;
			world.M41 = left;
			world.M42 = bottom;


			Matrix worldViewProjection;

			Matrix.Multiply(ref world, ref view, out worldViewProjection);
			Matrix.Multiply(ref worldViewProjection, ref projection, out worldViewProjection);
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
			dataStream.Write(_color.Vector4);
			deviceContext.UnmapSubresource(_globalBuffer, 0);

			deviceContext.DrawIndexed(6, 0, 0);
		}
	}
}
