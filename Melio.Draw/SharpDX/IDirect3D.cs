namespace Melio.Draw.SharpDX
{
	public interface IDirect3D
	{
		int SavedMaxRenderTargetWidth { get; }
		int SavedMaxRenderTargetHeight { get; }
		void Reset(DrawEventArgs args);
		void Render(DrawEventArgs args);
		OrthogonalCamera Camera { get; set; }
	}
}
