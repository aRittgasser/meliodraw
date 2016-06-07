using System;
using System.Windows.Input;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using Melio.Draw.ViewModel;
using Melio.Draw.SharpDX;
using Melio.Draw.Extensions;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Point = System.Windows.Point;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Melio.Draw.Visible
{
	public class Neatline : Component
	{
		#region private member
#if DEBUG
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness | ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#else
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness;
#endif
		// Background
		private Buffer _globalBufferBackground;
		private VertexShader _vertexShaderBackground;
		private PixelShader _pixelShaderBackground;
		private InputLayout _inputLayoutBackground;

		private Buffer _bufferBackground;
		private Buffer _indexBufferBackground;
		private VertexBufferBinding _bufferBindingBackground;
		private Vector4 _color1Background;
		private Vector4 _color2Background;

		private const float borderWidth = 15.0f;

		// Lines
		private Buffer _globalBufferLines;
		private VertexShader _vertexShaderLines;
		private PixelShader _pixelShaderLines;
		private InputLayout _inputLayoutLines;

		// Scala Lines
		private Buffer _bufferScalaLines;
		private VertexBufferBinding _bufferBindingScalaLines;
		private Vector4 _colorScalaLines;
		private const int _countScalaElements = 100;

		// Boundary Lines
		private Buffer _bufferBoundaryLines;
		private VertexBufferBinding _bufferBindingBoundaryLines;
		private Vector4 _colorBoundaryLines;

		// Cursor Lines
		private Buffer _globalBufferCursorLines;
		private VertexShader _vertexShaderCursorLines;
		private PixelShader _pixelShaderCursorLines;
		private InputLayout _inputLayoutCursorLines;

		private Buffer _bufferCursorLines;
		private VertexBufferBinding _bufferBindingCursorLines;
		private Vector4 _colorCursorLines;
		private bool _drawCursorLines = false;


		// Scala Numbers
		Numbers _numbersRenderer;
		private Vector4 _colorNumbers;
		//Font Font;
		//FontShader FontShader;


		#endregion

		public Neatline(Device device)
		{
			_color1Background = new Vector4(1.0f, 1.0f, 1.0f, 0.8f);
			_color2Background = new Vector4(0.7f, 0.7f, 0.7f, 0.8f);
			_colorScalaLines = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
			_colorBoundaryLines = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
			_colorNumbers = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
			#region BoundaryLines and Scala Lines
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Visible\Neatline.Background.hlsl", "VS", "vs_5_0", shaderFlags, EffectFlags.None))
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Neatline.Background.hlsl", "VS", "vs_5_0"))
			{
				_vertexShaderBackground = ToDispose(new VertexShader(device, bytecode));
				_inputLayoutBackground = ToDispose(new InputLayout(device, bytecode, new[] { new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0) }));

			}
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Neatline.Background.hlsl", "PS", "ps_5_0"))
			{
				_pixelShaderBackground = ToDispose(new PixelShader(device, bytecode));
			}

			_globalBufferBackground = ToDispose(new Buffer(device, Matrix.SizeInBytes + 4* Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Visible\Neatline.Lines.hlsl", "VS", "vs_5_0", shaderFlags, EffectFlags.None))
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Neatline.Lines.hlsl", "VS", "vs_5_0"))
			{
				_vertexShaderLines = ToDispose(new VertexShader(device, bytecode));
				_inputLayoutLines = ToDispose(new InputLayout(device, bytecode, new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0) }));
			}
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Visible\Neatline.Lines.hlsl", "PS", "ps_5_0", shaderFlags, EffectFlags.None))
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Neatline.Lines.hlsl", "PS", "ps_5_0"))
			{
				_pixelShaderLines = ToDispose(new PixelShader(device, bytecode));
			}

			_globalBufferLines = ToDispose(new Buffer(device, Matrix.SizeInBytes + Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

			using (var dataStream = new DataStream(Vector4.SizeInBytes * 12, true, true))
			{
				// Left Side
				dataStream.Write(new Vector4(0.0f, 1.0f, 0.0f, 0.0f));
				dataStream.Write(new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
				dataStream.Write(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
				dataStream.Write(new Vector4(1.0f, 1.0f, 0.0f, 0.0f));

				// Top Side
				dataStream.Write(new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
				dataStream.Write(new Vector4(1.0f, 1.0f, 0.0f, 0.0f));
				dataStream.Write(new Vector4(0.0f, 1.0f, 1.0f, 0.0f));
				dataStream.Write(new Vector4(0.0f, 0.0f, 1.0f, 0.0f));

				// Top-Left Corner
				dataStream.Write(new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
				dataStream.Write(new Vector4(1.0f, 1.0f, 0.0f, 0.0f));
				dataStream.Write(new Vector4(0.0f, 1.0f, 0.0f, 0.0f));
				dataStream.Write(new Vector4(0.0f, 0.0f, 0.0f, 0.0f));

				dataStream.Position = 0;
				_bufferBackground = ToDispose(new Buffer(device, dataStream, Vector4.SizeInBytes * 12, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				_bufferBindingBackground = new VertexBufferBinding(_bufferBackground, Vector4.SizeInBytes, 0);
			}
			using (var dataStream = new DataStream(sizeof(int) * 18, true, true))
			{
				dataStream.Write(0);
				dataStream.Write(2);
				dataStream.Write(1);
				dataStream.Write(2);
				dataStream.Write(0);
				dataStream.Write(3);

				dataStream.Write(4);
				dataStream.Write(6);
				dataStream.Write(5);
				dataStream.Write(6);
				dataStream.Write(4);
				dataStream.Write(7);

				dataStream.Write(12);
				dataStream.Write(14);
				dataStream.Write(13);
				dataStream.Write(14);
				dataStream.Write(12);
				dataStream.Write(17);
				dataStream.Position = 0;
				_indexBufferBackground = ToDispose(new Buffer(device, dataStream, sizeof(int) * 18, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
			}
			// Scala Lines
			using (var dataStream = new DataStream(Vector3.SizeInBytes * 20* _countScalaElements, true, true))
			{
				for (int i = 0; i < _countScalaElements; i++) {
					for(int j= 0; j < 10; j++)
					{
						dataStream.Write(new Vector3(0.1f*j + 1.0f * i, 0.0f, 0.0f));
						dataStream.Write(new Vector3(0.1f*j + 1.0f * i, j==0?1.0f : 0.3f, 0.0f));
					}
				}

				dataStream.Position = 0;
				_bufferScalaLines = ToDispose(new Buffer(device, dataStream, Vector3.SizeInBytes * 20 * _countScalaElements, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				_bufferBindingScalaLines = new VertexBufferBinding(_bufferScalaLines, Vector3.SizeInBytes, 0);
			}
			// Boundary Lines
			using (var dataStream = new DataStream(Vector3.SizeInBytes * 5, true, true))
			{
				dataStream.Write(new Vector3(0.0f, 0.0f, 0.0f));
				dataStream.Write(new Vector3(0.0f, 1.0f, 0.0f));
				dataStream.Write(new Vector3(1.0f, 1.0f, 0.0f));
				dataStream.Write(new Vector3(1.0f, 0.0f, 0.0f));
				dataStream.Write(new Vector3(0.0f, 0.0f, 0.0f));
				dataStream.Position = 0;
				_bufferBoundaryLines = ToDispose(new Buffer(device, dataStream, Vector3.SizeInBytes * 5, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				_bufferBindingBoundaryLines= new VertexBufferBinding(_bufferBoundaryLines, Vector3.SizeInBytes, 0);
			}
			#endregion
			#region Cursor Lines
			_colorCursorLines = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Neatline.CursorLines.hlsl", "VS", "vs_5_0"))
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Visible\Neatline.CursorLines.hlsl", "VS", "vs_5_0", shaderFlags, EffectFlags.None))
			{
				_vertexShaderCursorLines= ToDispose(new VertexShader(device, bytecode));
				_inputLayoutCursorLines = ToDispose(new InputLayout(device, bytecode, new[] { new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0) }));
			}
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Visible\Neatline.CursorLines.hlsl", "PS", "ps_5_0", shaderFlags, EffectFlags.None))
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Neatline.CursorLines.hlsl", "PS", "ps_5_0"))
			{
				_pixelShaderCursorLines= ToDispose(new PixelShader(device, bytecode));
			}

			_globalBufferCursorLines = ToDispose(new Buffer(device, Matrix.SizeInBytes + 2 * Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
			using (var dataStream = new DataStream(Vector4.SizeInBytes * 4, true, true))
			{
				// Top Side
				dataStream.Write(new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
				dataStream.Write(new Vector4(0.0f, 1.0f, 1.0f, 0.0f));
				// Left Side
				dataStream.Write(new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
				dataStream.Write(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));

				dataStream.Position = 0;
				_bufferCursorLines = ToDispose(new Buffer(device, dataStream, Vector4.SizeInBytes * 4, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				_bufferBindingCursorLines = new VertexBufferBinding(_bufferCursorLines, Vector4.SizeInBytes, 0);
			}
			#endregion

			_numbersRenderer = new Numbers();
			_numbersRenderer.Init(device);

		}

		public void Render(DeviceContext deviceContext, ProjectMV project)
		{
			var camera = project.Camera;
			float bottom = camera.ProjectY(0.0f);
			float top = camera.ProjectY(project.Height);
			float left = camera.ProjectX(0.0f);
			float right = camera.ProjectX(project.Width);

			Matrix linesWorld;
			Matrix projection = camera.Projection;
			Matrix worldViewProjection;
			DataStream dataStream;

			#region Boundary Lines

			deviceContext.InputAssembler.InputLayout = _inputLayoutLines;
			deviceContext.VertexShader.Set(_vertexShaderLines);
			deviceContext.HullShader.Set(null);
			deviceContext.DomainShader.Set(null);
			deviceContext.PixelShader.Set(_pixelShaderLines);

			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;
			deviceContext.InputAssembler.SetVertexBuffers(0, _bufferBindingBoundaryLines);

			deviceContext.VertexShader.SetConstantBuffers(0, _globalBufferLines);
			deviceContext.PixelShader.SetConstantBuffers(0, _globalBufferLines);

			linesWorld = Matrix.Identity;
			linesWorld.M11 = right - left;
			linesWorld.M22 = -top + bottom;
			linesWorld.M41 = left - camera.Width / 2.0f;
			linesWorld.M42 = -bottom + camera.Height / 2.0f;

			Matrix.Multiply(ref linesWorld, ref projection, out worldViewProjection);
			// Transpose local Matrices
			Matrix.Transpose(ref worldViewProjection, out worldViewProjection);

			dataStream = null;
			deviceContext.MapSubresource(_globalBufferLines, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.Write(worldViewProjection);
			dataStream.Write(_colorBoundaryLines);
			deviceContext.UnmapSubresource(_globalBufferLines, 0);

			deviceContext.Draw(5, 0);

			#endregion
			#region  Render Background
			Vector4 workingRect = new Vector4(bottom, top, left, right);
			Vector4 metrics = new Vector4(borderWidth, -borderWidth, camera.Width, -camera.Height);


			Matrix backgroundWorld = Matrix.Identity;
			backgroundWorld.M41 = -camera.Width / 2.0f;
			backgroundWorld.M42 = camera.Height / 2.0f;


			Matrix.Multiply(ref backgroundWorld, ref projection, out worldViewProjection);
			// Transpose local Matrices
			Matrix.Transpose(ref worldViewProjection, out worldViewProjection);

			deviceContext.VertexShader.SetConstantBuffers(0, _globalBufferBackground);
			deviceContext.PixelShader.SetConstantBuffers(0, _globalBufferBackground);

			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			deviceContext.InputAssembler.InputLayout = _inputLayoutBackground;
			deviceContext.VertexShader.Set(_vertexShaderBackground);
			deviceContext.HullShader.Set(null);
			deviceContext.DomainShader.Set(null);
			deviceContext.PixelShader.Set(_pixelShaderBackground);

			deviceContext.InputAssembler.SetIndexBuffer(_indexBufferBackground, Format.R32_UInt, 0);
			deviceContext.InputAssembler.SetVertexBuffers(0, _bufferBindingBackground);

			dataStream= null;
			deviceContext.MapSubresource(_globalBufferBackground, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.Write(worldViewProjection);
			dataStream.Write(_color1Background);
			dataStream.Write(_color2Background);
			dataStream.Write(workingRect);
			dataStream.Write(metrics);
			deviceContext.UnmapSubresource(_globalBufferBackground, 0);

			deviceContext.DrawIndexed(18, 0, 0);

			#endregion
			#region Scala Lines
			deviceContext.VertexShader.SetConstantBuffers(0, _globalBufferLines);
			deviceContext.PixelShader.SetConstantBuffers(0, _globalBufferLines);

			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
			deviceContext.InputAssembler.InputLayout = _inputLayoutLines;
			deviceContext.VertexShader.Set(_vertexShaderLines);
			deviceContext.HullShader.Set(null);
			deviceContext.DomainShader.Set(null);
			deviceContext.PixelShader.Set(_pixelShaderLines);

			deviceContext.InputAssembler.SetVertexBuffers(0, _bufferBindingScalaLines);
			
			// Top Lines
			linesWorld = Matrix.Identity;
            linesWorld.M42 = camera.Height / 2.0f - 15.0f;
			linesWorld.M22 = 10.0f;
			// Compute Zoom 
			
			float level;
			float scala;
			computeScala(camera.Zoom, out scala, out level);

			float shiftStepsX = (float)Math.Floor(left / scala);
            float shiftX = shiftStepsX * scala;
			int endIndex = Math.Min((int) (camera.Width / scala), _countScalaElements)+2;
			//int beginIndex = 20 - (int)((left- shift) / 10.0f);
			//int beginIndex = 20 - (int)((left- shift) * scala / 1000.0f);
			int beginIndex = 0;

			linesWorld.M11 = scala;
			linesWorld.M41 = -camera.Width / 2.0f + left- shiftX - scala;

			Matrix.Multiply(ref linesWorld, ref projection, out worldViewProjection);
			// Transpose local Matrices
			Matrix.Transpose(ref worldViewProjection, out worldViewProjection);

			dataStream = null;
			deviceContext.MapSubresource(_globalBufferLines, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.Write(worldViewProjection);
			dataStream.Write(_colorScalaLines);
			deviceContext.UnmapSubresource(_globalBufferLines, 0);

			deviceContext.Draw(20* endIndex, beginIndex*2);


			// Left Lines
			float shiftStepsY = (float)Math.Floor((camera.Height - bottom) / scala + 1.0f);
            float shiftY = shiftStepsY * scala;
			endIndex = Math.Min((int)(camera.Height / scala), _countScalaElements) + 2;

			linesWorld = Matrix.Identity;
			linesWorld.M42 = +camera.Height / 2.0f - bottom - shiftY;
			linesWorld.M12 = scala;


			linesWorld.M41 = -camera.Width / 2.0f + 15.0f;
			linesWorld.M11 = 0.0f;
			linesWorld.M22 = 0.0f;
			linesWorld.M21 = -10.0f;

			Matrix.Multiply(ref linesWorld, ref projection, out worldViewProjection);
			// Transpose local Matrices
			Matrix.Transpose(ref worldViewProjection, out worldViewProjection);

			dataStream = null;
			deviceContext.MapSubresource(_globalBufferLines, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
			dataStream.Write(worldViewProjection);
			dataStream.Write(_colorScalaLines);
			deviceContext.UnmapSubresource(_globalBufferLines, 0);

			deviceContext.Draw(20 * endIndex, 0);
			#endregion
			#region Curser Lines
			if (_drawCursorLines)
			{
				//Vector2 curserpos = camera.UnProject(project.CurserPos);

				metrics = new Vector4(borderWidth, -borderWidth, (float)project.CurserPos.X, -(float)project.CurserPos.Y);
				Matrix cursorWorld = Matrix.Identity;
				cursorWorld.M41 = -camera.Width / 2.0f;
				cursorWorld.M42 = camera.Height / 2.0f;


				Matrix.Multiply(ref cursorWorld, ref projection, out worldViewProjection);
				// Transpose local Matrices
				Matrix.Transpose(ref worldViewProjection, out worldViewProjection);

				deviceContext.VertexShader.SetConstantBuffers(0, _globalBufferCursorLines);
				deviceContext.PixelShader.SetConstantBuffers(0, _globalBufferCursorLines);

				deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
				deviceContext.InputAssembler.InputLayout = _inputLayoutCursorLines;
				deviceContext.VertexShader.Set(_vertexShaderCursorLines);
				deviceContext.HullShader.Set(null);
				deviceContext.DomainShader.Set(null);
				deviceContext.PixelShader.Set(_pixelShaderCursorLines);

				deviceContext.InputAssembler.SetVertexBuffers(0, _bufferBindingCursorLines);

				dataStream = null;
				deviceContext.MapSubresource(_globalBufferCursorLines, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
				dataStream.Write(worldViewProjection);
				dataStream.Write(_colorCursorLines);
				dataStream.Write(metrics);
				deviceContext.UnmapSubresource(_globalBufferCursorLines, 0);

				deviceContext.Draw(4, 0);
			}
			#endregion
			#region Top Numbers
			int digits = Math.Max(-(int)Math.Floor(Math.Log10(level))+0,0);
			float valueX = -shiftStepsX*level;

			float firstX = left - shiftX;
			int countX = (int)((camera.Width+30.0f) / scala);

			for(int i = 0; i< countX; i++)
			{
				float posX = firstX + 5.0f + scala*i;
				_numbersRenderer.Draw(valueX, posX, camera.Height-10.0f, deviceContext, camera, digits);
				valueX += level;
            }
			// Left Numbers
			float valueY = -shiftStepsY * level;

			float firstY = (bottom - shiftY);
			int countY = (int)((camera.Height + 30.0f) / scala);

			//for (int i = 0; i < countY; i++)
			//{
			//	float posY = firstY - scala * i;
			//	//_numbersRenderer.Draw(valueY, 12.0f, -posY, deviceContext, camera, true);
			//	_numbersRenderer.Draw(valueY, 12.0f, camera.Height/2.0f - posY - 10.0f, deviceContext, camera, true);

			//	valueY += level;
			//}
			for (int i = 1; i < countY+2; i++)
			{
				_numbersRenderer.Draw(level * (-shiftStepsY + i), 10.0f, camera.Height - (bottom + shiftY) + 5.0f + scala*i, deviceContext, camera, digits, true);
			}
			#endregion
		}

		private void computeScala(float zoom, out float scala, out float level)
		{
			const float minScala = 150.0f;
			float[] levels = { 1.0f, 2.0f, 5.0f};


			// scala = zoom*level
			// scale > minScale;
			// find the smales scala where level in levels*10^n
			// level = scala/zoom > minScala/zoom
			float maxLevel = minScala / zoom;
			float pow10 = (float)Math.Log10(maxLevel);
			float pow10Floor = (float)Math.Floor(pow10);
			float rpow10 = pow10 - pow10Floor;
			if (rpow10 > (float)Math.Log10(5.0))
			{
				level = 5.0f * (float)Math.Pow(10.0, pow10Floor);
            }
			else if (rpow10 > (float)Math.Log10(2.0))
			{
				level = 2.0f * (float)Math.Pow(10.0, pow10Floor);
			}
			else
			{
				level = 1.0f * (float)Math.Pow(10.0, pow10Floor);
			}

			scala = zoom * level;
        }
    }

	class Numbers : Component
	{
		public class GlyphInfo
		{
			public float Height, Width;
			public float Position;
		}

#if DEBUG
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness | ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#else
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness;
#endif

		private Dictionary<char, GlyphInfo> _glyphs;
		private const float texWidth = 57.0f;
		private const float texHeight = 9.0f;

		private Buffer _globalBuffer;

		private VertexShader _vertexShader;
		private PixelShader _pixelShader;
		private InputLayout _inputLayout;

		private Buffer _buffer;
		private Buffer _indexBuffer;
		private VertexBufferBinding _bufferBinding;

		private ShaderResourceView _texture = null;
		SamplerState _sampleState;

		private Vector4 _color;

		public Numbers()
		{
			_glyphs = new Dictionary<char, GlyphInfo>();

			_glyphs['0'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 1.0f };
			_glyphs['1'] = new GlyphInfo() { Height = 7.0f, Width = 3.0f, Position = 0.5f + 5.0f };
			_glyphs['2'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 9.0f };
			_glyphs['3'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 14.0f };
			_glyphs['4'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 19.0f };
			_glyphs['5'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 24.0f };
			_glyphs['6'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 29.0f };
			_glyphs['7'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 34.0f };
			_glyphs['8'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 39.0f };
			_glyphs['9'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 44.0f };
			_glyphs['-'] = new GlyphInfo() { Height = 7.0f, Width = 4.0f, Position = 0.5f + 49.0f };
			_glyphs[','] = new GlyphInfo() { Height = 7.0f, Width = 1.0f, Position = 0.5f + 54.0f };
			_glyphs['.'] = new GlyphInfo() { Height = 7.0f, Width = 1.0f, Position = 0.5f + 54.0f };
		}

		~Numbers()
		{
			if(_texture != null)
			{
				_texture.Dispose();
				_texture = null;
            }
		}

		public void Init(Device device)
		{
			_color = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
			using (Stream stream = Application.GetResourceStream(new Uri("pack://application:,,,/Content/Images/numbers.png")).Stream)
            {
				_texture = ShaderResourceView.FromStream(device, stream, (int)stream.Length);
            }

			//_texture = ShaderResourceView.FromFile(device, @"Content\Images\numbers.png");
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Neatline.Numbers.hlsl", "VS", "vs_5_0"))
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Visible\Neatline.Numbers.hlsl", "VS", "vs_5_0", shaderFlags, EffectFlags.None))
			{
				_vertexShader = ToDispose(new VertexShader(device, bytecode));
				_inputLayout = ToDispose(new InputLayout(device, bytecode, new[] { new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0) }));
			}
			using (var bytecode = ShaderBytecodeExtension.CompileFromResource("pack://application:,,,/Shader/Visible/Neatline.Numbers.hlsl", "PS", "ps_5_0"))
			//using (var bytecode = ShaderBytecode.CompileFromFile(@"Shader\Visible\Neatline.Numbers.hlsl", "PS", "ps_5_0", shaderFlags, EffectFlags.None))
			{
				_pixelShader = ToDispose(new PixelShader(device, bytecode));
			}

			_globalBuffer = ToDispose(new Buffer(device, Matrix.SizeInBytes + 2*Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

			using (var dataStream = new DataStream(Vector4.SizeInBytes * 4, true, true))
			{
				dataStream.Write(new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
				dataStream.Write(new Vector4(0.0f, 1.0f, 0.0f, 0.0f));
				dataStream.Write(new Vector4(1.0f, 1.0f, 1.0f, 0.0f));
				dataStream.Write(new Vector4(1.0f, 0.0f, 1.0f, 1.0f));

				dataStream.Position = 0;
				_buffer = ToDispose(new Buffer(device, dataStream, Vector4.SizeInBytes * 4, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
				_bufferBinding = new VertexBufferBinding(_buffer, Vector4.SizeInBytes, 0);
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

			// Create a texture sampler state description.
			var samplerDesc = new SamplerStateDescription()
			{
				//Filter = Filter.MinMagMipLinear,
				Filter = Filter.MinMagMipPoint,
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				MipLodBias = 0,
				MaximumAnisotropy = 1,
				ComparisonFunction = Comparison.Always,
				BorderColor = new Color4(0, 0, 0, 0),
				MinimumLod = 0,
				MaximumLod = 0
			};

			// Create the texture sampler state.
			_sampleState = new SamplerState(device, samplerDesc);
		}

		public float CaclulateWidth(string text)
		{
			float width = 0.0f;
			foreach (char c in text)
			{
				if (!_glyphs.ContainsKey(c))
					continue;
				var glyph = _glyphs[c];
				width += glyph.Width + 1.0f;
			}
			return width;
		}

		public void Draw(float number, float posX, float posY, DeviceContext deviceContext, OrthogonalCamera camera, int digits=2, bool vertical = false)
		{
			//string text = string.Format("{0:n1}", number);
			string text = string.Format(string.Format("{0:s}0:n{1:d}{2:s}", "{", digits, "}"), number);
			Draw(text, posX, posY, deviceContext, camera, vertical);
        }

		public void Draw(string text, float posX, float posY, DeviceContext deviceContext, OrthogonalCamera camera, bool vertical = false)
		{
			Matrix view = Matrix.Identity;
			Matrix projection = camera.Projection;

			deviceContext.PixelShader.SetShaderResource(0, _texture);
			deviceContext.VertexShader.SetConstantBuffers(0, _globalBuffer);
			deviceContext.PixelShader.SetConstantBuffers(0, _globalBuffer);

			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			deviceContext.InputAssembler.InputLayout = _inputLayout;
			deviceContext.VertexShader.Set(_vertexShader);
			deviceContext.HullShader.Set(null);
			deviceContext.DomainShader.Set(null);
			deviceContext.PixelShader.Set(_pixelShader);
			deviceContext.PixelShader.SetSampler(0, _sampleState);

			deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
			deviceContext.InputAssembler.SetVertexBuffers(0, _bufferBinding);

			float shift = 0.0f;
			//if (vertical)
			//	posY += CaclulateWidth(text);

			foreach (char c in text)
			{
				if (!_glyphs.ContainsKey(c))
					continue;

				var glyph = _glyphs[c];

				Vector4 texPosition = new Vector4(glyph.Position / texWidth, 1.0f / texWidth, (glyph.Width+1.0f)/texWidth, (glyph.Height+1.0f)/ texHeight);

				Matrix world = Matrix.Identity;
				if (!vertical)
				{
					world.M11 = glyph.Width + 1.0f;
					world.M22 = glyph.Height + 1.0f;
					world.M41 = (float)Math.Floor(posX + shift - camera.Width / 2.0f);
					world.M42 = (float)Math.Floor(posY - camera.Height / 2.0f);
				}
				else
				{
					texPosition = new Vector4((glyph.Position-0.5f) / texWidth, 1.0f / texWidth, (glyph.Width + 1.0f) / texWidth, (glyph.Height + 1.0f) / texHeight);
					world.M11 = 0.0f;
					world.M22 = 0.0f;
                    world.M21 = -(glyph.Height + 1.0f);
					world.M12 = glyph.Width + 1.0f ;
					world.M41 = (float)Math.Floor(posX - camera.Width / 2.0f);
					world.M42 = (float)Math.Floor(posY + shift - camera.Height / 2.0f);
				}
				shift += glyph.Width + 1.0f;

				Matrix worldViewProjection;

				Matrix.Multiply(ref world, ref view, out worldViewProjection);
				Matrix.Multiply(ref worldViewProjection, ref projection, out worldViewProjection);
				// Transpose local Matrices
				Matrix.Transpose(ref worldViewProjection, out worldViewProjection);

				DataStream dataStream;
				deviceContext.MapSubresource(_globalBuffer, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
				dataStream.Write(worldViewProjection);
				dataStream.Write(_color);
				dataStream.Write(texPosition);
				deviceContext.UnmapSubresource(_globalBuffer, 0);

				deviceContext.DrawIndexed(6, 0, 0);
			}
		}
	}
}
