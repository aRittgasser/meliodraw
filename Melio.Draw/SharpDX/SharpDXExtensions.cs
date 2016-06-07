using System;
using System.IO;
using System.Text;
using SharpDX;

namespace Melio.Draw.SharpDX
{

	public static class SharpDXExtensions
	{

		public static T GetOrThrow<T>(this T obj)
			where T : class, IDisposable
		{
			if (obj == null)
				throw new ObjectDisposedException(typeof(T).Name);
			return obj;
		}

		public static short ReadInt16(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 2, false), Encoding.UTF8, false))
				return reader.ReadInt16();
		}

		public static int ReadInt32(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 4, false), Encoding.UTF8, false))
				return reader.ReadInt32();
		}

		public static long ReadInt64(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 8, false), Encoding.UTF8, false))
				return reader.ReadInt64();
		}

		public static float ReadHalf(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 2, false), Encoding.UTF8, false))
				return new Half(reader.ReadUInt16());
		}

		public static float ReadSingle(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 4, false), Encoding.UTF8, false))
				return reader.ReadSingle();
		}

		public static Vector2 ReadVector2(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 8, false), Encoding.UTF8, false))
				return new Vector2(reader.ReadSingle(), reader.ReadSingle());
		}

		public static Vector3 ReadVector3(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 12, false), Encoding.UTF8, false))
				return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Vector4 ReadVector4(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 16, false), Encoding.UTF8, false))
				return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Color3 ReadColor3(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 12, false), Encoding.UTF8, false))
				return new Color3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Color4 ReadColor4(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 16, false), Encoding.UTF8, false))
				return new Color4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Quaternion ReadQuaternion(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 16, false), Encoding.UTF8, false))
				return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Matrix ReadMatrix(this byte[] buffer, int start = 0)
		{
			using (var reader = new BinaryReader(new MemoryStream(buffer, start, 64, false), Encoding.UTF8, false))
				return new Matrix(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static void Write(this byte[] buffer, short value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 2, true), Encoding.UTF8, false))
				writer.Write(value);
		}

		public static void Write(this byte[] buffer, int value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 4, true), Encoding.UTF8, false))
				writer.Write(value);
		}

		public static void Write(this byte[] buffer, long value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 8, true), Encoding.UTF8, false))
				writer.Write(value);
		}

		public static void Write(this byte[] buffer, Half value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 2, true), Encoding.UTF8, false))
				writer.Write(value.RawValue);
		}

		public static void Write(this byte[] buffer, float value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 4, true), Encoding.UTF8, false))
				writer.Write(value);
		}

		public static void Write(this byte[] buffer, Vector2 value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 8, true), Encoding.UTF8, false))
			{
				writer.Write(value.X);
				writer.Write(value.Y);
			}
		}

		public static void Write(this byte[] buffer, Vector3 value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 12, true), Encoding.UTF8, false))
			{
				writer.Write(value.X);
				writer.Write(value.Y);
				writer.Write(value.Z);
			}
		}

		public static void Write(this byte[] buffer, Vector4 value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 16, true), Encoding.UTF8, false))
			{
				writer.Write(value.X);
				writer.Write(value.Y);
				writer.Write(value.Z);
				writer.Write(value.W);
			}
		}

		public static void Write(this byte[] buffer, Color3 value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 12, true), Encoding.UTF8, false))
			{
				writer.Write(value.Red);
				writer.Write(value.Green);
				writer.Write(value.Blue);
			}
		}

		public static void Write(this byte[] buffer, Color4 value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 16, true), Encoding.UTF8, false))
			{
				writer.Write(value.Red);
				writer.Write(value.Green);
				writer.Write(value.Blue);
				writer.Write(value.Alpha);
			}
		}

		public static void Write(this byte[] buffer, Quaternion value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 16, true), Encoding.UTF8, false))
			{
				writer.Write(value.X);
				writer.Write(value.Y);
				writer.Write(value.Z);
				writer.Write(value.W);
			}
		}

		public static void Write(this byte[] buffer, Matrix value, int start = 0)
		{
			using (var writer = new BinaryWriter(new MemoryStream(buffer, start, 16, true), Encoding.UTF8, false))
			{
				writer.Write(value.M11);
				writer.Write(value.M12);
				writer.Write(value.M13);
				writer.Write(value.M14);
				writer.Write(value.M21);
				writer.Write(value.M22);
				writer.Write(value.M23);
				writer.Write(value.M24);
				writer.Write(value.M31);
				writer.Write(value.M32);
				writer.Write(value.M33);
				writer.Write(value.M34);
				writer.Write(value.M41);
				writer.Write(value.M42);
				writer.Write(value.M43);
				writer.Write(value.M44);
			}
		}

	}

}
