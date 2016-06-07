using SharpDX.D3DCompiler;
using System;
using System.IO;
using System.Windows;

namespace Melio.Draw.Extensions
{
	public static class ShaderBytecodeExtension
	{
#if DEBUG
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness | ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#else
		private const ShaderFlags shaderFlags = ShaderFlags.EnableStrictness;
#endif

		public static CompilationResult CompileFromResource(string resourceName, string entryPoint, string profile, EffectFlags effectFlags = EffectFlags.None, string sourceFileName = "unknown")
		{

			using (Stream stream = Application.GetResourceStream(new Uri(resourceName)).Stream)
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					return ShaderBytecode.Compile(reader.ReadToEnd(), entryPoint, profile, shaderFlags, effectFlags, sourceFileName);
				}
			}
		}
	}
}
