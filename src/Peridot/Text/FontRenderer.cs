// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using FontStashSharp;
using FontStashSharp.Interfaces;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace Peridot
{
	internal class FontStashRenderer : IFontStashRenderer, IDisposable
	{
		private bool m_disposed;
		private readonly ISpriteBatch m_spriteBatch;
		private readonly Texture2DManager m_textureManager;
		public FontStashRenderer(IPeridot peridot, ISpriteBatch spriteBatch)
		{
			m_spriteBatch = spriteBatch;
			m_textureManager = new Texture2DManager(peridot);
			ResetScissor();
		}

		~FontStashRenderer()
		{
			Dispose(false);
		}

		public Rectangle Scissor { get; set; }

		public ITexture2DManager TextureManager => m_textureManager;
		
		private void Dispose(bool disposing)
		{
			if (m_disposed)
				return;
			m_disposed = true;
			if (disposing)
				m_textureManager.Dispose();
		}
		
		public void Dispose()
		{
			Dispose(true);
		}

		public void ResetScissor()
		{
			const int v = 1 << 23;
			const int s = -(1 << 22);
			Scissor = new Rectangle(s, s, v, v);
		}

		public void Draw(object texture, Vector2 pos, Rectangle? src, FSColor color, float rotation, Vector2 scale, float depth)
		{
			Debug.Assert(texture is Image, "The texture object must be a Image.");
			if (texture is not Image t)
				throw new ArgumentException(nameof(texture));

			var s = src ?? new(0, 0, (int)t.Width, (int)t.Height);

			var oldScissor = m_spriteBatch.Scissor;
			m_spriteBatch.Scissor = RectangleF.Intersect(m_spriteBatch.Scissor, Scissor);
			m_spriteBatch.Draw(t, pos, s, ToColor(color), rotation, Vector2.Zero, new Vector2(scale.X, scale.Y), depth);
			m_spriteBatch.Scissor = oldScissor;
		}
		
		private static Color ToColor(in FSColor color)
		{
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		private class Texture2DManager : ITexture2DManager, IDisposable
		{
			private readonly List<WeakReference<Image>> m_images;
			private readonly IPeridot m_peridot;
			private bool m_disposed;
			public Texture2DManager(IPeridot peridot)
			{
				m_images = new();
				m_peridot = peridot;
				m_disposed = false;
			}
			
			~Texture2DManager()
			{
				Dispose(false);
			}
			
			public object CreateTexture(int width, int height)
			{
				var image = m_peridot.CreateImage(new ImageDescription()
				{
					Format = PixelFormat.BGRA8,
					Width = (uint)width,
					Height = (uint)height,
				});
				m_images.Add(new WeakReference<Image>(image));
				return image;
			}

			public Point GetTextureSize(object texture)
			{
				Debug.Assert(texture is Image);
				Image image = (texture as Image)!;
				return new Point(image.Width, image.Height);
			}

			public void SetTextureData(object texture, Rectangle bounds, byte[] data)
			{
				Debug.Assert(texture is Image);
				
				if (texture is not Image)
					throw new NotImplementedException();
				
				m_peridot.UpdateImage<byte>(
					(texture as Image)!,
					data, 
					(uint)bounds.X,
					(uint)bounds.Y,
					(uint)bounds.Width,
					(uint)bounds.Height);
			}
			
			public void Dispose()
			{
				Dispose(true);
			}

			private void Dispose(bool disposing)
			{
				if (m_disposed)
					return;
				m_disposed = true;

				if (disposing)
				{
					foreach (var wt in m_images)
						if (wt.TryGetTarget(out var texture))
							texture.Dispose();
				}
			}
		}
	}
}
