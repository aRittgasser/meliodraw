using System;
using System.Collections.Generic;
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
using Melio.Draw.ViewModel;

namespace Melio.Draw.Models
{


	public class Polygon : BaseModel
	{
		public class Vertex
		{
			public float X, Y;
			public int Id;
		}
		public class Triangle
		{
			public int V1, V2, V3;

			public Triangle(int v1, int v2, int v3)
			{
				V1 = v1;
				V2 = v2;
				V3 = v3;
			}

			public Triangle(Vertex v1, Vertex v2, Vertex v3)
			{
				V1 = v1.Id;
				V2 = v2.Id;
				V3 = v3.Id;
			}

			public static bool Contains(ICollection<Vertex> points, Vertex t1, Vertex t2, Vertex t3)
			{
				foreach(var point in points)
				{
					if (Contains(point, t1, t2, t3))
						return true;
				}
				return false;
			}
            public static bool Contains(Vertex p, Vertex t1, Vertex t2, Vertex t3)
			{
				// spanning vectors from t1
				float ax = t2.X - t1.X;
				float ay = t2.Y - t1.Y;
				float bx = t3.X - t1.X;
				float by = t3.Y - t1.Y;
				float b = bx / by;
				float px = p.X - t1.X;
				float py = p.Y - t1.Y;
				// Find u and v for p = u*a+v*b
				float u = (px - py * b) / (ax - ay * b);
				float v = (py - u * ay) / by;
				return (u > 0.0f && v > 0.0f) && (u + v < 1.0f);
			}

			public static bool Contains(Vector2 p, Vertex t1, Vertex t2, Vertex t3)
			{
				// spanning vectors from t1
				float ax = t2.X - t1.X;
				float ay = t2.Y - t1.Y;
				float bx = t3.X - t1.X;
				float by = t3.Y - t1.Y;
				float b = bx / by;
				float px = p.X - t1.X;
				float py = p.Y - t1.Y;
				// Find u and v for p = u*a+v*b
				float u = (px - py * b) / (ax - ay * b);
				float v = (py - u * ay) / by;
				return (u > 0.0f && v > 0.0f) && (u + v < 1.0f);
			}

			public static bool IsLH(Vertex t1, Vertex t2, Vertex t3)
			{
				// Check Keil product (check if '>' or '<')
				return (t1.X-t2.X) * (t2.Y-t3.Y) - (t2.X-t3.X) * (t1.Y-t2.Y) < 0;
			}
		}
		public class Edge
		{
			public int I1, I2;
			public Vertex V1, V2;
			/// <summary>
			/// Doeses the intersect to right. (increasing x)
			/// </summary>
			/// <param name="origX">The original x.</param>
			/// <param name="origY">The original y.</param>
			/// <returns></returns>
			public bool DoesIntersectToRight(float origX, float origY)
			{
				if ((V1.Y > origY && V2.Y > origY) || (V1.Y < origY && V2.Y < origY))
					return false;
				if (V1.X < origX && V2.X < origX)
					return false;
				if (V1.X > origX && V2.X > origX)
					return true;
				if (V1.Y - V2.Y == 0) // -> origY = V1.Y
					return true; 
				float d1 = Math.Abs(origY - V1.Y) / Math.Abs(V1.Y - V2.Y);
				float d2 = Math.Abs(origY - V2.Y) / Math.Abs(V1.Y - V2.Y);
				float x = d1 * V1.X + d2 * V2.X;
				return x > origX;
            }
		}

		private class ShaderManager : Component
        {
			private Buffer _globalBuffer;
			private VertexShader _vertexShader;
			private PixelShader _pixelShader;
			private InputLayout _inputLayout;
			private Device _device;

			public Buffer GlobalBuffer { get { return _globalBuffer; } }
			public VertexShader VertexShader { get { return _vertexShader; } }
			public PixelShader PixelShader { get { return _pixelShader; } }
			public InputLayout InputLayout { get { return _inputLayout; } }
			public Device Device { get { return _device; } }

			public void Init(D3D11 d3d)
			{
				_device = d3d.Device;
				using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Models.Polygon.hlsl", "VS", "vs_5_0"))
				//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Models.Polygon.hlsl", "VS", "vs_5_0", shaderFlags, EffectFlags.None))
				{
					_vertexShader = ToDispose(new VertexShader(_device, bytecode));
					_inputLayout = ToDispose(new InputLayout(_device, bytecode, new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0) }));
				}
				//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Models.Polygon.hlsl", "PS", "ps_5_0", shaderFlags, EffectFlags.None))
				using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Models.Polygon.hlsl", "PS", "ps_5_0"))

				{
					_pixelShader = ToDispose(new PixelShader(_device, bytecode));
				}

				_globalBuffer = ToDispose(new Buffer(_device, Matrix.SizeInBytes + Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
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

		#region private fields

#if DEBUG
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness | ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#else
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness;
#endif


		private static int _polygonCounter = 0;
		private Buffer _buffer;
		private Buffer _indexBuffer;
		private VertexBufferBinding _bufferBinding;

		//private Vector4 _color;
		public ColorVM Color;
		#endregion

		private List<Vertex> _vertices;
		private List<Triangle> _triangles;
		private List<Edge> _edges;

		public Polygon(Vector2 position, List<Vertex> vertices, ColorVM color, string label) : base(label, position, Vector2.One)
		{
			_vertices = vertices;
			//_color = new Vector4(0.9f, 0.0f, 0.0f, 0.4f);
			Color = new ColorVM(color);
			// Tesslate the path
			_triangles = new List<Triangle>();
			_edges = new List<Edge>();
			tesslate();
			if (_triangles.Count == 0)
				return;

			var device = ShaderManager.Instance.Device;

			using (var dataStream = new DataStream(Vector3.SizeInBytes * _vertices.Count, true, true))
			{
				foreach (var vertex in _vertices)
				{
					dataStream.Write(new Vector3(vertex.X, vertex.Y, 1.0f));
				}

				dataStream.Position = 0;
				_buffer = ToDispose(new Buffer(device, dataStream, Vector3.SizeInBytes * _vertices.Count, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				_bufferBinding = new VertexBufferBinding(_buffer, Vector3.SizeInBytes, 0);
			}
			using (var dataStream = new DataStream(sizeof(int) * 3 * _triangles.Count, true, true))
			{
				foreach (var triangle in _triangles)
				{
					dataStream.Write(triangle.V1);
					dataStream.Write(triangle.V2);
					dataStream.Write(triangle.V3);
				}
				dataStream.Position = 0;
				_indexBuffer = ToDispose(new Buffer(device, dataStream, sizeof(int) * 3 * _triangles.Count, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
			}
			updateBoundingBox();
		}

		public Polygon(Vector2 position, List<Vertex> vertices, ColorVM color) : base(string.Format("Polygon{0}", _polygonCounter++), position, Vector2.One)
		{
			_vertices = vertices;
			//_color = new Vector4(0.9f, 0.0f, 0.0f, 0.4f);
			Color = new ColorVM(color);
			// Tesslate the path
			_triangles = new List<Triangle>();
			_edges = new List<Edge>();
			tesslate();
			if (_triangles.Count == 0)
				return;

			var device = ShaderManager.Instance.Device;

			using (var dataStream = new DataStream(Vector3.SizeInBytes * _vertices.Count, true, true))
			{
				foreach (var vertex in _vertices)
				{
					dataStream.Write(new Vector3(vertex.X, vertex.Y, 1.0f));
				}

				dataStream.Position = 0;
				_buffer = ToDispose(new Buffer(device, dataStream, Vector3.SizeInBytes * _vertices.Count, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				_bufferBinding = new VertexBufferBinding(_buffer, Vector3.SizeInBytes, 0);
			}
			using (var dataStream = new DataStream(sizeof(int) * 3 * _triangles.Count, true, true))
			{
				foreach (var triangle in _triangles)
				{
					dataStream.Write(triangle.V1);
					dataStream.Write(triangle.V2);
					dataStream.Write(triangle.V3);
				}
				dataStream.Position = 0;
				_indexBuffer = ToDispose(new Buffer(device, dataStream, sizeof(int) * 3* _triangles.Count, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
			}
			updateBoundingBox();
		}

		public static void Init(D3D11 d3d)
		{
			ShaderManager.Instance.Init(d3d);
		}

		public override void RenderContent(DeviceContext deviceContext, OrthogonalCamera camera)
		{
			Matrix view = camera.View;
			Matrix projection = camera.Projection;

			//Matrix world = Matrix.Identity;



			//world.M41 = - camera.Width / 2.0f;
			//world.M42 = - camera.Height / 2.0f;
			//world.M42 = + camera.Height / 2.0f;

			Matrix worldViewProjection;

			Matrix.Multiply(ref _world, ref view, out worldViewProjection);
			Matrix.Multiply(ref worldViewProjection, ref projection, out worldViewProjection);
			// Transpose local Matrices
			Matrix.Transpose(ref worldViewProjection, out worldViewProjection);

			deviceContext.VertexShader.SetConstantBuffers(0, ShaderManager.Instance.GlobalBuffer);
			deviceContext.PixelShader.SetConstantBuffers(0, ShaderManager.Instance.GlobalBuffer);

			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			deviceContext.InputAssembler.InputLayout = ShaderManager.Instance.InputLayout;
			deviceContext.VertexShader.Set(ShaderManager.Instance.VertexShader);
			deviceContext.HullShader.Set(null);
			deviceContext.DomainShader.Set(null);
			deviceContext.PixelShader.Set(ShaderManager.Instance.PixelShader);

			deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
			deviceContext.InputAssembler.SetVertexBuffers(0, _bufferBinding);

			DataStream dataStream;
			deviceContext.MapSubresource(ShaderManager.Instance.GlobalBuffer, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.Write(worldViewProjection);
			dataStream.Write(Color.Vector4);
			deviceContext.UnmapSubresource(ShaderManager.Instance.GlobalBuffer, 0);

			deviceContext.DrawIndexed(3* _triangles.Count, 0, 0);
		}

		public override bool Hit(Vector2 position)
		{
			// translation
			position -= _position;
			// rotation
			// TODO
			// scalation
			position /= _scale;
			// Test bounding boxes
			if (position.X < _left ||
				position.X > _right ||
				position.Y < _bottom ||
				position.Y > _top)
				return false;

			foreach (var tri in _triangles)
			{
				if (!Triangle.Contains(position, _vertices[tri.V1], _vertices[tri.V2], _vertices[tri.V3]))
					return true;
			}
			return false;
		}

		private void tesslate()
		{
			// Doing Ear Clipping. No Holes Allowd!

			// Computing Edges
			for (int i = 0; i < _vertices.Count - 1; i++)
			{
				_edges.Add(new Edge() { I1 = i, I2 = i + 1, V1 = _vertices[i], V2 = _vertices[i + 1] });
			}
			_edges.Add(new Edge() { I1 = _vertices.Count - 1, I2 = 0, V1 = _vertices[_vertices.Count-1], V2 = _vertices[0] });

			var todo = new List<Vertex>();
			int c = 0;
			foreach (var vertex in _vertices)
			{
				vertex.Id = c++;
                todo.Add(vertex);
            }
			int steps = 0;
			while (todo.Count > 2 && steps++< _vertices.Count)
			{
				for (int i = 0; i < todo.Count - 2; i++)
				{
					Vertex t1 = todo[i];
					Vertex t2 = todo[i+1];
					Vertex t3 = todo[i+2];

					if (Triangle.Contains(_vertices, t1, t2, t3))
						continue;
					// Do not check convexity yet
                    if (Triangle.IsLH(t1, t2, t3))
					{
						todo.RemoveAt(i+1);
						_triangles.Add(new Triangle(t1, t2, t3));
					}
					else
					{
						//_triangles.Add(new Triangle(t1, t2, t3));
					}

				}
			}
		}
		/// <summary>
		/// Updates the bounding box.
		/// TODO: Note the rounded edges
		/// </summary>
		protected override void updateBoundingBox()
		{
			_left = 999.99f;
			_right= -999.99f;
			_bottom = 999.99f;
			_top = -999.99f;
			foreach(var vertex in _vertices)
			{
				_left = Math.Min(_left, vertex.X);
				_right = Math.Max(_right, vertex.X);

				_bottom = Math.Min(_bottom, vertex.Y);
				_top = Math.Max(_top, vertex.Y);
			}
			_size.X = _right - _left;
			_size.Y = _top - _bottom;
        }
	}
}
