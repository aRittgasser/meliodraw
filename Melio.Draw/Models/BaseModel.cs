using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Melio.Draw.SharpDX;
using Melio.Draw.Extensions;

using Component = SharpDX.Component;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Melio.Draw.Models
{
	public abstract class BaseModel : Component, INotifyPropertyChanged
	{
		private class ShaderManager : Component
		{
			private Device _device;
			public Device Device { get { return _device; } }
			#region Boundary Rect
			private Buffer _globalBufferBoundingRect;
			private VertexShader _vertexShaderBoundingRect;
			private PixelShader _pixelShaderBoundingRect;
			private InputLayout _inputLayoutBoundingRect;

			private Buffer _bufferBoundingRect;
			private VertexBufferBinding _bufferBindingBoundingRect;
			private Vector4 _colorBoundingRect;

			public Buffer GlobalBufferBoundingRect { get { return _globalBufferBoundingRect; } }
			public VertexShader VertexShaderBoundingRect { get { return _vertexShaderBoundingRect; } }
			public PixelShader PixelShaderBoundingRect { get { return _pixelShaderBoundingRect; } }
			public InputLayout InputLayoutBoundingRect { get { return _inputLayoutBoundingRect; } }

			public Buffer BufferBoundingRect {get { return _bufferBoundingRect; }}
			public VertexBufferBinding BufferBindingBoundingRect{get { return _bufferBindingBoundingRect; }}
			public Vector4 ColorBoundingRect {get { return _colorBoundingRect; }}
			#endregion

			#region Nodes
			private Buffer _globalBufferNodes;
			private VertexShader _vertexShaderNodes;
			private PixelShader _pixelShaderNodes;
			private InputLayout _inputLayoutNodes;

			private Buffer _bufferNodes;
			private Buffer _indexBufferNodesLines;
			private Buffer _indexBufferNodesTriangles;
			private VertexBufferBinding _bufferBindingNodes;
			private Vector4 _colorNodesBorders;
			private Vector4 _colorNodesFill;

			public Buffer GlobalBufferNodes { get { return _globalBufferNodes; } }
			public VertexShader VertexShaderNodes { get { return _vertexShaderNodes; } }
			public PixelShader PixelShaderNodes { get { return _pixelShaderNodes; } }
			public InputLayout InputLayoutNodes { get { return _inputLayoutNodes; } }

			public Buffer BufferNodes { get { return _bufferNodes; } }
			public Buffer IndexBufferNodesLines { get { return _indexBufferNodesLines; } }
			public Buffer IndexBufferNodesTriangles { get { return _indexBufferNodesTriangles; } }
			public VertexBufferBinding BufferBindingNodes { get { return _bufferBindingNodes; } }
			public Vector4 ColorNodesBorder { get { return _colorNodesBorders; } }
			public Vector4 ColorNodesFill { get { return _colorNodesFill; } }
			#endregion


			public void Init(D3D11 d3d)
			{
				_device = d3d.Device;

				#region BoundingRect
				using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/BaseModel/BoundingRect.hlsl", "VS", "vs_5_0"))
				{
					_vertexShaderBoundingRect = ToDispose(new VertexShader(_device, bytecode));
					_inputLayoutBoundingRect = ToDispose(new InputLayout(_device, bytecode, new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0) }));
				}
				using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/BaseModel/BoundingRect.hlsl", "PS", "ps_5_0"))

				{
					_pixelShaderBoundingRect = ToDispose(new PixelShader(_device, bytecode));
				}

				_globalBufferBoundingRect = ToDispose(new Buffer(_device, Matrix.SizeInBytes + Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

				_colorBoundingRect = new Vector4(100.0f/255.0f, 149.0f/255.0f, 237.0f/255.0f, 1.0f);

				using (var dataStream = new DataStream(Vector3.SizeInBytes * 5, true, true))
				{
					dataStream.Write(new Vector3(0.0f, 0.0f, 0.0f));
					dataStream.Write(new Vector3(0.0f, 1.0f, 0.0f));
					dataStream.Write(new Vector3(1.0f, 1.0f, 0.0f));
					dataStream.Write(new Vector3(1.0f, 0.0f, 0.0f));
					dataStream.Write(new Vector3(0.0f, 0.0f, 0.0f));
					dataStream.Position = 0;
					_bufferBoundingRect = ToDispose(new Buffer(_device, dataStream, Vector3.SizeInBytes * 5, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
					_bufferBindingBoundingRect = new VertexBufferBinding(_bufferBoundingRect, Vector3.SizeInBytes, 0);
				}
				#endregion
				#region Nodes

				_colorNodesBorders = new Vector4(100.0f / 255.0f, 149.0f / 255.0f, 237.0f / 255.0f, 1.0f);
				_colorNodesFill = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
				using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/BaseModel/BoundingNodes.hlsl", "VS", "vs_5_0"))
				{
					_vertexShaderNodes = ToDispose(new VertexShader(_device, bytecode));
					_inputLayoutNodes = ToDispose(new InputLayout(_device, bytecode, new[] { new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0) }));
				}
				using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/BaseModel/BoundingNodes.hlsl", "PS", "ps_5_0"))

				{
					_pixelShaderNodes = ToDispose(new PixelShader(_device, bytecode));
				}

				_globalBufferNodes = ToDispose(new Buffer(_device, 2*Matrix.SizeInBytes + 3 * Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

				using (var dataStream = new DataStream(Vector4.SizeInBytes * 32, true, true))
				{
					dataStream.Write(new Vector4(-0.5f, -0.5f, -0.5f, -0.5f));
					dataStream.Write(new Vector4(-0.5f, +0.5f, -0.5f, -0.5f));
					dataStream.Write(new Vector4(+0.5f, +0.5f, -0.5f, -0.5f));
					dataStream.Write(new Vector4(+0.5f, -0.5f, -0.5f, -0.5f));

					dataStream.Write(new Vector4(-0.5f, -0.5f, 0.0f, -0.5f));
					dataStream.Write(new Vector4(-0.5f, +0.5f, 0.0f, -0.5f));
					dataStream.Write(new Vector4(+0.5f, +0.5f, 0.0f, -0.5f));
					dataStream.Write(new Vector4(+0.5f, -0.5f, 0.0f, -0.5f));

					dataStream.Write(new Vector4(-0.5f, -0.5f, 0.5f, -0.5f));
					dataStream.Write(new Vector4(-0.5f, +0.5f, 0.5f, -0.5f));
					dataStream.Write(new Vector4(+0.5f, +0.5f, 0.5f, -0.5f));
					dataStream.Write(new Vector4(+0.5f, -0.5f, 0.5f, -0.5f));

					dataStream.Write(new Vector4(-0.5f, -0.5f, -0.5f, 0.0f));
					dataStream.Write(new Vector4(-0.5f, +0.5f, -0.5f, 0.0f));
					dataStream.Write(new Vector4(+0.5f, +0.5f, -0.5f, 0.0f));
					dataStream.Write(new Vector4(+0.5f, -0.5f, -0.5f, 0.0f));

					dataStream.Write(new Vector4(-0.5f, -0.5f, 0.5f, 0.0f));
					dataStream.Write(new Vector4(-0.5f, +0.5f, 0.5f, 0.0f));
					dataStream.Write(new Vector4(+0.5f, +0.5f, 0.5f, 0.0f));
					dataStream.Write(new Vector4(+0.5f, -0.5f, 0.5f, 0.0f));

					dataStream.Write(new Vector4(-0.5f, -0.5f, -0.5f, +0.5f));
					dataStream.Write(new Vector4(-0.5f, +0.5f, -0.5f, +0.5f));
					dataStream.Write(new Vector4(+0.5f, +0.5f, -0.5f, +0.5f));
					dataStream.Write(new Vector4(+0.5f, -0.5f, -0.5f, +0.5f));

					dataStream.Write(new Vector4(-0.5f, -0.5f, 0.0f, +0.5f));
					dataStream.Write(new Vector4(-0.5f, +0.5f, 0.0f, +0.5f));
					dataStream.Write(new Vector4(+0.5f, +0.5f, 0.0f, +0.5f));
					dataStream.Write(new Vector4(+0.5f, -0.5f, 0.0f, +0.5f));

					dataStream.Write(new Vector4(-0.5f, -0.5f, 0.5f, +0.5f));
					dataStream.Write(new Vector4(-0.5f, +0.5f, 0.5f, +0.5f));
					dataStream.Write(new Vector4(+0.5f, +0.5f, 0.5f, +0.5f));
					dataStream.Write(new Vector4(+0.5f, -0.5f, 0.5f, +0.5f));

					dataStream.Position = 0;
					_bufferNodes = ToDispose(new Buffer(_device, dataStream, Vector4.SizeInBytes * 32, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
					_bufferBindingNodes = new VertexBufferBinding(_bufferNodes, Vector4.SizeInBytes, 0);
				}

				using (var dataStream = new DataStream(sizeof(int) * 64, true, true))
				{
					dataStream.Write(0+0);
					dataStream.Write(1+0);
					dataStream.Write(1+0);
					dataStream.Write(2+0);
					dataStream.Write(2+0);
					dataStream.Write(3+0);
					dataStream.Write(3+0);
					dataStream.Write(0+0);

					dataStream.Write(0+4);
					dataStream.Write(1+4);
					dataStream.Write(1+4);
					dataStream.Write(2+4);
					dataStream.Write(2+4);
					dataStream.Write(3+4);
					dataStream.Write(3+4);
					dataStream.Write(0+4);

					dataStream.Write(0+8);
					dataStream.Write(1+8);
					dataStream.Write(1+8);
					dataStream.Write(2+8);
					dataStream.Write(2+8);
					dataStream.Write(3+8);
					dataStream.Write(3+8);
					dataStream.Write(0+8);

					dataStream.Write(0 + 12);
					dataStream.Write(1 + 12);
					dataStream.Write(1 + 12);
					dataStream.Write(2 + 12);
					dataStream.Write(2 + 12);
					dataStream.Write(3 + 12);
					dataStream.Write(3 + 12);
					dataStream.Write(0 + 12);

					dataStream.Write(0 + 16);
					dataStream.Write(1 + 16);
					dataStream.Write(1 + 16);
					dataStream.Write(2 + 16);
					dataStream.Write(2 + 16);
					dataStream.Write(3 + 16);
					dataStream.Write(3 + 16);
					dataStream.Write(0 + 16);

					dataStream.Write(0 + 20);
					dataStream.Write(1 + 20);
					dataStream.Write(1 + 20);
					dataStream.Write(2 + 20);
					dataStream.Write(2 + 20);
					dataStream.Write(3 + 20);
					dataStream.Write(3 + 20);
					dataStream.Write(0 + 20);

					dataStream.Write(0 + 24);
					dataStream.Write(1 + 24);
					dataStream.Write(1 + 24);
					dataStream.Write(2 + 24);
					dataStream.Write(2 + 24);
					dataStream.Write(3 + 24);
					dataStream.Write(3 + 24);
					dataStream.Write(0 + 24);

					dataStream.Write(0 + 28);
					dataStream.Write(1 + 28);
					dataStream.Write(1 + 28);
					dataStream.Write(2 + 28);
					dataStream.Write(2 + 28);
					dataStream.Write(3 + 28);
					dataStream.Write(3 + 28);
					dataStream.Write(0 + 28);
					dataStream.Position = 0;
					_indexBufferNodesLines = ToDispose(new Buffer(_device, dataStream, sizeof(int) * 64, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				}

				using (var dataStream = new DataStream(sizeof(int) * 6 * 8, true, true))
				{
					dataStream.Write(0 + 0);
					dataStream.Write(1 + 0);
					dataStream.Write(2 + 0);
					dataStream.Write(2 + 0);
					dataStream.Write(3 + 0);
					dataStream.Write(0 + 0);

					dataStream.Write(0 + 4);
					dataStream.Write(1 + 4);
					dataStream.Write(2 + 4);
					dataStream.Write(2 + 4);
					dataStream.Write(3 + 4);
					dataStream.Write(0 + 4);

					dataStream.Write(0 + 8);
					dataStream.Write(1 + 8);
					dataStream.Write(2 + 8);
					dataStream.Write(2 + 8);
					dataStream.Write(3 + 8);
					dataStream.Write(0 + 8);

					dataStream.Write(0 + 12);
					dataStream.Write(1 + 12);
					dataStream.Write(2 + 12);
					dataStream.Write(2 + 12);
					dataStream.Write(3 + 12);
					dataStream.Write(0 + 12);

					dataStream.Write(0 + 16);
					dataStream.Write(1 + 16);
					dataStream.Write(2 + 16);
					dataStream.Write(2 + 16);
					dataStream.Write(3 + 16);
					dataStream.Write(0 + 16);

					dataStream.Write(0 + 20);
					dataStream.Write(1 + 20);
					dataStream.Write(2 + 20);
					dataStream.Write(2 + 20);
					dataStream.Write(3 + 20);
					dataStream.Write(0 + 20);

					dataStream.Write(0 + 24);
					dataStream.Write(1 + 24);
					dataStream.Write(2 + 24);
					dataStream.Write(2 + 24);
					dataStream.Write(3 + 24);
					dataStream.Write(0 + 24);

					dataStream.Write(0 + 28);
					dataStream.Write(1 + 28);
					dataStream.Write(2 + 28);
					dataStream.Write(2 + 28);
					dataStream.Write(3 + 28);
					dataStream.Write(0 + 28);

					dataStream.Position = 0;
					_indexBufferNodesTriangles = ToDispose(new Buffer(_device, dataStream, sizeof(int) * 6*8, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				}

				#endregion
			}
			#region Singleton
			private ShaderManager() { }

			public static ShaderManager Instance
			{
				get
				{
					if (_instance == null)
						_instance = new ShaderManager();
					return _instance;
				}
			}

			private static ShaderManager _instance = null;
			#endregion
		}

		private static int _unknownBaseModelCounter = 0;
		private static DXElement _dxElement = null;
		#region protected Members
		protected Vector2 _position;
		protected Vector2 _scale;
		protected Vector2 _center;
		protected Vector2 _size; // Unscaled
		protected float _rotation;
		protected bool _isSelected;
		/// <summary>
		/// The Bounding box
		/// </summary>
		protected float _left, _right, _top, _bottom;
		protected Matrix _world;
		protected string _label;
		#endregion
		public new event PropertyChangedEventHandler PropertyChanged;

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			// Render
			if(_dxElement != null)
			_dxElement.Render();
        }
		#region Properties

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged();
				}
			}
		}

		public string Label
		{
			get { return _label; }
			set
			{
				if (_label != value)
				{
					_label = value;
					OnPropertyChanged();
				}
			}
		}

		public Vector2 Size
		{
			get
			{
				return _size;
			}
		}

		public Vector2 Center
		{
            get {
				return _center;
			}
			set
			{
				if (_center != value)
				{
					_center = value;
					updateScaleAndRotation();
					OnPropertyChanged();
				}
			}
		}

		public Vector2 Position
		{
			get { return _position; }
			set
			{
				if (_position != value)
				{
					_position = value;
					updateTranslation();
					OnPropertyChanged();
				}
			}
		}

		public Vector2 Scale
		{
			get { return _scale; }
			set
			{
				if (_scale != value)
				{
					_scale = value;
					updateScaleAndRotation();
					OnPropertyChanged();
				}
			}
		}
		public float Rotation
		{
			get { return _rotation; }
			set
			{
				if (_rotation != value)
				{
					_rotation = value;
					updateScaleAndRotation();
					OnPropertyChanged();
				}
			}
		}
		#endregion
		#region Constructors
		public BaseModel(string label, Vector2 position, Vector2 scale, float rotation = 0.0f)
		{
			_label = label;
			_world = Matrix.Identity;
			_position = position;
			_scale = scale;
			_rotation = rotation;
			updateWorldMatrix();
			//updateBoundingBox();
        }

		public BaseModel(Vector2 position, Vector2 scale, float rotation = 0.0f)
		{
			_label = string.Format("Model{0}", _unknownBaseModelCounter++);
			_world = Matrix.Identity;
			_position = position;
			_scale = scale;
			_rotation = rotation;
			updateWorldMatrix();
			//updateBoundingBox();
		}
		#endregion
		public abstract void RenderContent(DeviceContext deviceContext, OrthogonalCamera camera);
		//public void Render(DeviceContext deviceContext, OrthogonalCamera camera)
		//{
		//	RenderContent(deviceContext, camera);
		//	if(_isSelected)
		//		RenderBoundingBox(deviceContext, camera);
		//}

		public abstract bool Hit(Vector2 position);

		public Vector2 BottomLeft
		{
			get
			{
				Vector2 result = new Vector2(_left, _bottom);
				// translate
				result += _position;
				// scale
				result *= _scale;
				//rotate
				//TODO
				return result;
			}
		}
		public Vector2 TopLeft
		{
			get
			{
				Vector2 result = new Vector2(_left, _top);
				// translate
				result += _position;
				// scale
				result *= _scale;
				//rotate
				//TODO
				return result;
			}
		}
		public Vector2 TopRight
		{
			get
			{
				Vector2 result = new Vector2(_right, _top);
				// translate
				result += _position;
				// scale
				result *= _scale;
				//rotate
				//TODO
				return result;
			}
		}
		public Vector2 BottomRight
		{
			get
			{
				Vector2 result = new Vector2(_right, _bottom);
				// translate
				result += _position;
				// scale
				result *= _scale;
				//rotate
				//TODO
				return result;
			}
		}

		public Vector2 Right
		{
			get
			{
				Vector2 result = new Vector2(_right, (_top + _bottom) / 2.0f);
				// translate
				result += _position;
				// scale
				result *= _scale;
				//rotate
				//TODO
				return result;
			}
		}
		public Vector2 Left
		{
			get
			{
				Vector2 result = new Vector2(_left, (_top + _bottom) / 2.0f);
				// translate
				result += _position;
				// scale
				result *= _scale;
				//rotate
				//TODO
				return result;
			}
		}
		public Vector2 Bottom
		{
			get
			{
				Vector2 result = new Vector2((_left+ _right) /2.0f, _bottom);
				// translate
				result += _position;
				// scale
				result *= _scale;
				//rotate
				//TODO
				return result;
			}
		}
		public Vector2 Top
		{
			get
			{
				Vector2 result = new Vector2((_left + _right) / 2.0f, _top);
				// translate
				result += _position;
				// scale
				result *= _scale;
				//rotate
				//TODO
				return result;
			}
		}

		protected void updateTranslation()
		{
			_world.M41 = _position.X;
			_world.M42 = _position.Y;
		}

		protected void updateScaleAndRotation()
		{
			// TODO _center of Rotation
			_world.M11 = _scale.X * (float)Math.Cos(_rotation);
			_world.M12 = -_scale.X * (float)Math.Sin(_rotation);
			_world.M21 = _scale.Y * (float)Math.Sin(_rotation);
			_world.M22 = _scale.Y * (float)Math.Cos(_rotation);
		}

		protected void updateWorldMatrix()
		{
			updateScaleAndRotation();
			updateTranslation();
        }
		protected abstract void updateBoundingBox();

		public void RenderBoundingBox(DeviceContext deviceContext, OrthogonalCamera camera)
		{
			Matrix view = camera.View;
			Matrix projection = camera.Projection;
			//Matrix linesWorld;
			Matrix worldViewProjection;
			DataStream dataStream;

			Matrix world = _world;
			world.M41 += _left* world.M11;
			world.M42 += _bottom* world.M22;
			world.M11 *= _right - _left;
			world.M22 *= _top - _bottom;

			#region Boundary Rect // Correct
			deviceContext.InputAssembler.InputLayout = ShaderManager.Instance.InputLayoutBoundingRect;
			deviceContext.VertexShader.Set(ShaderManager.Instance.VertexShaderBoundingRect);
			deviceContext.HullShader.Set(null);
			deviceContext.DomainShader.Set(null);
			deviceContext.PixelShader.Set(ShaderManager.Instance.PixelShaderBoundingRect);

			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;
			deviceContext.InputAssembler.SetVertexBuffers(0, ShaderManager.Instance.BufferBindingBoundingRect);

			deviceContext.VertexShader.SetConstantBuffers(0, ShaderManager.Instance.GlobalBufferBoundingRect);
			deviceContext.PixelShader.SetConstantBuffers(0, ShaderManager.Instance.GlobalBufferBoundingRect);

			Matrix.Multiply(ref world, ref view, out worldViewProjection);
			Matrix.Multiply(ref worldViewProjection, ref projection, out worldViewProjection);

			dataStream = null;
			deviceContext.MapSubresource(ShaderManager.Instance.GlobalBufferBoundingRect, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.Write(worldViewProjection);
			dataStream.Write(ShaderManager.Instance.ColorBoundingRect);
			deviceContext.UnmapSubresource(ShaderManager.Instance.GlobalBufferBoundingRect, 0);

			deviceContext.Draw(5, 0);
			#endregion
			#region Nodes
			deviceContext.InputAssembler.InputLayout = ShaderManager.Instance.InputLayoutNodes;
			deviceContext.VertexShader.Set(ShaderManager.Instance.VertexShaderNodes);
			deviceContext.HullShader.Set(null);
			deviceContext.DomainShader.Set(null);
			deviceContext.PixelShader.Set(ShaderManager.Instance.PixelShaderNodes);

			deviceContext.InputAssembler.SetVertexBuffers(0, ShaderManager.Instance.BufferBindingNodes);

			deviceContext.VertexShader.SetConstantBuffers(0, ShaderManager.Instance.GlobalBufferNodes);
			deviceContext.PixelShader.SetConstantBuffers(0, ShaderManager.Instance.GlobalBufferNodes);

			//Vector2 position = camera.Project(_position);
			//float posX = camera.ProjectX(_position.X);
			//float posY = camera.ProjectY(_position.Y);
			Matrix worldView;
			world = _world;
			//world.M11 *= _right - _left;
			//world.M22 *= _top - _bottom;

			//float width = (float)Math.Floor(_right - _left);
			//float height = (float)Math.Floor(_top - _bottom);
			float width = _right - _left;
			float height = _top - _bottom;

			Vector4 metric = new Vector4(6.0f, 6.0f, Math.Max(18.0f, width), Math.Max(18.0f, height));
			Vector4 rect = new Vector4(_left+ width / 2.0f, _bottom + height / 2.0f, Math.Max(18.0f, width), Math.Max(18.0f, height));

			Matrix.Multiply(ref world, ref view, out worldView);
			//Matrix.Multiply(ref worldViewProjection, ref projection, out worldViewProjection);

			#region Lines
			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
			deviceContext.InputAssembler.SetIndexBuffer(ShaderManager.Instance.IndexBufferNodesLines, Format.R32_UInt, 0);

			dataStream = null;
			deviceContext.MapSubresource(ShaderManager.Instance.GlobalBufferNodes, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.Write(worldView);
			dataStream.Write(projection);
			dataStream.Write(ShaderManager.Instance.ColorNodesBorder);
			dataStream.Write(metric);
			dataStream.Write(rect);
			deviceContext.UnmapSubresource(ShaderManager.Instance.GlobalBufferNodes, 0);

			deviceContext.DrawIndexed(8*8, 0, 0);
			#endregion

			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			deviceContext.InputAssembler.SetIndexBuffer(ShaderManager.Instance.IndexBufferNodesTriangles, Format.R32_UInt, 0);
			metric.X -= 1.0f;
			metric.Y -= 1.0f;
            dataStream = null;
			deviceContext.MapSubresource(ShaderManager.Instance.GlobalBufferNodes, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.Write(worldView);
			dataStream.Write(projection);
			dataStream.Write(ShaderManager.Instance.ColorNodesFill);
			dataStream.Write(metric);
			dataStream.Write(rect);
			deviceContext.UnmapSubresource(ShaderManager.Instance.GlobalBufferNodes, 0);

			deviceContext.DrawIndexed(6 * 8, 0, 0);

			#endregion
		}

		public static void Init(D3D11 d3d, DXElement dxElement)
		{
			ShaderManager.Instance.Init(d3d);
			_dxElement = dxElement;
        }
	}
}
