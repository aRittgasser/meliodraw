using System;
using System.Windows;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Component = SharpDX.Component;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Melio.Draw.SharpDX
{

	public abstract class D3D11
		: Component, IDirect3D
	{

		#region Variablen

#if DEBUG
		private const DeviceCreationFlags deviceCreationFlags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug;
#else
		private const DeviceCreationFlags deviceCreationFlags = DeviceCreationFlags.BgraSupport;
#endif

		private Device device;
		private Texture2D renderTarget;
		private Texture2D renderTargetCpuCopy;
		private RenderTargetView renderTargetView;
		private Texture2D depthStencilBuffer;
		private DepthStencilView depthStencilView;

		private DepthStencilState depthEnabledStencilState;
		private DepthStencilState depthDisabledStencilState;
		private BlendState alphaEnabledBlendingState;
		private BlendState alphaDisabledBlendingState;
		private RasterizerState solidRasterizerState;
		private RasterizerState wireframeRasterizerState;
		private SamplerState linearSamplerState;
		private SamplerState pointSamplerState;

		private OrthogonalCamera camera;

		private bool zBufferEnabled;
		private bool alphaBlendingEnabled;
		private bool wireframeEnabled;

		#endregion

		#region Eigenschaften

		public Device Device { get { return device.GetOrThrow(); } }
		public OrthogonalCamera Camera { get { return camera; } set { camera = value; OnPropertyChanged("Camera"); } }
		public int SavedMaxRenderTargetWidth { get; private set; }
		public int SavedMaxRenderTargetHeight { get; private set; }
		public Vector2 RenderSize { get; private set; }
		public TimeSpan RenderTime { get; private set; }

		public SamplerState LinearSamplerState { get { return linearSamplerState; } }
		public SamplerState PointSamplerState { get { return pointSamplerState; } }

		public bool ZBufferEnabled
		{
			get { return zBufferEnabled; }
			set
			{
				if (value)
				{
					device.GetOrThrow().ImmediateContext.OutputMerger.SetDepthStencilState(depthEnabledStencilState, 1);
				}
				else
				{
					device.GetOrThrow().ImmediateContext.OutputMerger.SetDepthStencilState(depthDisabledStencilState, 1);
				}
				zBufferEnabled = value;
			}
		}

		public bool AlphaBlendingEnabled
		{
			get { return alphaBlendingEnabled; }
			set
			{
				if (value)
				{
					device.GetOrThrow().ImmediateContext.OutputMerger.SetBlendState(alphaEnabledBlendingState, new Color4(0.0f, 0.0f, 0.0f, 0.0f), 0xffffffff);
				}
				else
				{
					device.GetOrThrow().ImmediateContext.OutputMerger.SetBlendState(alphaDisabledBlendingState, new Color4(0.0f, 0.0f, 0.0f, 0.0f), 0xffffffff);
				}
				alphaBlendingEnabled = value;
			}
		}

		public bool WireframeEnabled
		{
			get { return wireframeEnabled; }
			set
			{
				if (value)
				{
					device.GetOrThrow().ImmediateContext.Rasterizer.State = wireframeRasterizerState;
				}
				else
				{
					device.GetOrThrow().ImmediateContext.Rasterizer.State = solidRasterizerState;
				}
				wireframeEnabled = value;
			}
		}

		#endregion

		#region Ereignisse

		public event EventHandler<DrawEventArgs> Resetted;

		#endregion

		#region ctor

		static D3D11()
		{
		}

		public D3D11()
			: base("D3D11")
		{
			device = ToDispose(new Device(DriverType.Hardware, deviceCreationFlags, FeatureLevel.Level_11_0));
			var depthStencilStateDescription = new DepthStencilStateDescription()
			{
				IsDepthEnabled = false,
				DepthWriteMask = DepthWriteMask.Zero,
				DepthComparison = Comparison.Always,
				IsStencilEnabled = false,
				StencilReadMask = 0xff,
				StencilWriteMask = 0xff,
				FrontFace = new DepthStencilOperationDescription()
				{
					FailOperation = StencilOperation.Keep,
					DepthFailOperation = StencilOperation.Increment,
					PassOperation = StencilOperation.Keep,
					Comparison = Comparison.Always
				},
				BackFace = new DepthStencilOperationDescription()
				{
					FailOperation = StencilOperation.Keep,
					DepthFailOperation = StencilOperation.Decrement,
					PassOperation = StencilOperation.Keep,
					Comparison = Comparison.Always
				}
			};
			depthEnabledStencilState = ToDispose(new DepthStencilState(device, depthStencilStateDescription));
			depthStencilStateDescription.IsDepthEnabled = false;
			depthDisabledStencilState = ToDispose(new DepthStencilState(device, depthStencilStateDescription));

			var blendStateDescription = new BlendStateDescription();
			blendStateDescription.RenderTarget[0].IsBlendEnabled = true;
			blendStateDescription.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
			blendStateDescription.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
			//blendStateDescription.RenderTarget[0].BlendOperation = BlendOperation.Add;
			blendStateDescription.RenderTarget[0].BlendOperation = BlendOperation.Add;
			blendStateDescription.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
			blendStateDescription.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
			blendStateDescription.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
			blendStateDescription.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
			alphaEnabledBlendingState = ToDispose(new BlendState(device, blendStateDescription));
			blendStateDescription.RenderTarget[0].IsBlendEnabled = false;
			alphaDisabledBlendingState = ToDispose(new BlendState(device, blendStateDescription));

			var rasterizerStateDescription = new RasterizerStateDescription()
			{
				IsAntialiasedLineEnabled = false,
				CullMode = CullMode.Back,
				DepthBias = 0,
				DepthBiasClamp = 0.0f,
				IsDepthClipEnabled = true,
				FillMode = FillMode.Solid,
				IsFrontCounterClockwise = true,
				IsMultisampleEnabled = false,
				IsScissorEnabled = false,
				SlopeScaledDepthBias = 0.0f
			};
			solidRasterizerState = ToDispose(new RasterizerState(device, rasterizerStateDescription));
			rasterizerStateDescription.IsAntialiasedLineEnabled = true;
			rasterizerStateDescription.CullMode = CullMode.None;
			rasterizerStateDescription.FillMode = FillMode.Wireframe;
			wireframeRasterizerState = ToDispose(new RasterizerState(device, rasterizerStateDescription));

			var samplerStateDescription = new SamplerStateDescription()
			{
				Filter = Filter.MinMagMipLinear,
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				ComparisonFunction = Comparison.Never,
				MaximumAnisotropy = 16,
				MinimumLod = 0,
				MaximumLod = float.MaxValue
			};
			linearSamplerState = ToDispose(new SamplerState(device, samplerStateDescription));
			samplerStateDescription.Filter = Filter.MinMagMipPoint;
			pointSamplerState = ToDispose(new SamplerState(device, samplerStateDescription));

			SavedMaxRenderTargetWidth = 0;
			SavedMaxRenderTargetHeight = 0;
		}

		#endregion

		#region Funktionen

		protected override void Dispose(bool disposeManagedResources)
		{
			if (disposeManagedResources)
			{
				Set(ref renderTargetCpuCopy, null);
				Set(ref depthStencilView, null);
				Set(ref depthStencilBuffer, null);
				Set(ref renderTargetView, null);
				Set(ref renderTarget, null);
			}
			base.Dispose(disposeManagedResources);
		}

		private void BeginRender()
		{
			var deviceContext = device.GetOrThrow().ImmediateContext;
			deviceContext.ClearRenderTargetView(renderTargetView, new Color4(0.9f, 0.9f, 0.9f, 1.0f));
			deviceContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
		}

		private void EndRender(DrawEventArgs args)
		{
			var deviceContext = device.ImmediateContext;
			deviceContext.Flush();
			var rect = new Int32Rect(0, 0, (int)Math.Ceiling(args.RenderSize.Width), (int)Math.Ceiling(args.RenderSize.Height));
			deviceContext.CopyResource(renderTarget, renderTargetCpuCopy);
			var box = deviceContext.MapSubresource(renderTargetCpuCopy, 0, MapMode.Read, MapFlags.None);
			try
			{
				//System.Diagnostics.Debug.WriteLine("EndRender");
				// Texturen sind mit einem vielfachen von 64 Pixel in X- und Y-Richtung angelegt worden.
				// Die aktuelle tatsächliche Texturbreite befindet sich in savedMaxRenderTargetWidth
				args.Target.WritePixels(rect, box.DataPointer, SavedMaxRenderTargetWidth * SavedMaxRenderTargetHeight * 4, SavedMaxRenderTargetWidth * 4, 0, 0);
			}
			finally
			{
				deviceContext.UnmapSubresource(renderTargetCpuCopy, 0);
			}
		}

		public void Render(DrawEventArgs args)
		{
			RenderTime = args.Timer.TotalTime;
			//if(camera != null)
			//	camera.FrameMove(args.Timer);
			BeginRender();
			RenderScene(args);
			EndRender(args);
		}

		protected abstract void RenderScene(DrawEventArgs args);

		public void Reset(DrawEventArgs args)
		{
			int w = (int)Math.Ceiling(args.RenderSize.Width);
			int h = (int)Math.Ceiling(args.RenderSize.Height);
			if (w < 1 || h < 1)
				return;
			RenderSize = new Vector2(w, h);
			if (camera != null)
			{
				//camera.AspectRatio = (float)(args.RenderSize.Width / args.RenderSize.Height);
				camera.Width = (float)args.RenderSize.Width;
				camera.Height = (float)args.RenderSize.Height;
            }
			reset(w, h);
			var handler = Resetted;
			if (handler != null)
				handler(this, args);
			Render(args);
		}

		private void reset(int width, int height)
		{
			device.GetOrThrow();
			if (width < 1)
				throw new ArgumentOutOfRangeException("width");
			if (height < 1)
				throw new ArgumentOutOfRangeException("height");

			// Texturen werden mit einem vielfachen von 64 Pixel in X- und Y-Richtung angelegt.
			int width64 = (width % 64) == 0 ? width : (width + 64 - (width % 64));
			int height64 = (height % 64) == 0 ? height : (height + 64 - (height % 64));
			if (width64 > SavedMaxRenderTargetWidth || height64 > SavedMaxRenderTargetHeight)
			{
				SavedMaxRenderTargetWidth = Math.Max(width64, SavedMaxRenderTargetWidth);
				SavedMaxRenderTargetHeight = Math.Max(height64, SavedMaxRenderTargetHeight);

				var renderTargetDescription = new Texture2DDescription()
				{
					BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
					Format = Format.B8G8R8A8_UNorm,
					Width = SavedMaxRenderTargetWidth,
					Height = SavedMaxRenderTargetHeight,
					MipLevels = 1,
					SampleDescription = new SampleDescription(1, 0),
					Usage = ResourceUsage.Default,
					OptionFlags = ResourceOptionFlags.None,
					CpuAccessFlags = CpuAccessFlags.None,
					ArraySize = 1
				};
				var depthStencilDescription = new Texture2DDescription()
				{
					BindFlags = BindFlags.DepthStencil,
					Format = Format.D24_UNorm_S8_UInt,
					Width = SavedMaxRenderTargetWidth,
					Height = SavedMaxRenderTargetHeight,
					MipLevels = 1,
					SampleDescription = new SampleDescription(1, 0),
					Usage = ResourceUsage.Default,
					OptionFlags = ResourceOptionFlags.None,
					CpuAccessFlags = CpuAccessFlags.None,
					ArraySize = 1
				};

				Set(ref renderTarget, new Texture2D(device, renderTargetDescription));
				Set(ref renderTargetView, new RenderTargetView(device, renderTarget));
				Set(ref depthStencilBuffer, new Texture2D(device, depthStencilDescription));
				Set(ref depthStencilView, new DepthStencilView(device, depthStencilBuffer));

				// Modifiziere die BackBufferbeschreibung, um eine CPU-Copie zu erhalten.
				renderTargetDescription.Usage = ResourceUsage.Staging;
				renderTargetDescription.BindFlags = BindFlags.None;
				renderTargetDescription.CpuAccessFlags = CpuAccessFlags.Read;
				// CPU-Kopie des Backbuffers erzeugen.
				Set(ref renderTargetCpuCopy, new Texture2D(device, renderTargetDescription));
			}

			// Der Viewport wird natürlich auf die tatsächliche Größe festgelegt.
			device.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, width, height, 0.0f, 1.0f));
			device.ImmediateContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);
		}

		public static void Set<T>(ref T field, T newValue)
			where T : IDisposable
		{
			if (field != null)
				field.Dispose();
			field = newValue;
		}

		#endregion

	}

}
