using System.Numerics;
using FontStashSharp;

namespace Peridot.Text
{
	/// <summary>
	/// Represents a font that can be rendered with <see cref="TextRenderer"/>.
	/// </summary>
	public class Font
	{
		/// <summary>
		/// Creates a new instance of <see cref="Font"/>.
		/// </summary>
		public Font()
		{
			var settings = new FontSystemSettings
			{
				Effect = FontSystemEffect.None,
				EffectAmount = 2,
			};
			FontSystem = new FontSystem(settings);
		}
		
		/// <summary>
		/// Destroys font.
		/// </summary>
		~Font()
		{
			Dispose(false);
		}

		internal FontSystem FontSystem { get; }

		/// <summary>
		/// A bool indicating whether this instance has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Add a character font to the font.
		/// </summary>
		/// <param name="stream">The stream that contains the font.</param>
		public void AddFont(Stream stream)
		{
			FontSystem.AddFont(stream);
		}

		/// <summary>
		/// Add a character font to the font.
		/// </summary>
		/// <param name="data">The data that contains the font.</param>
		public void AddFont(byte[] data)
		{
			FontSystem.AddFont(data);
		}

		/// <summary>
		/// Gets the size of a string when rendered in this font.
		/// </summary>
		/// <param name="text">The text to measure.</param>
		/// <param name="fontSize">The font size to measure.</param>
		/// <returns>The size of <paramref name="text"/> rendered with <paramref name="fontSize"/> font size.</returns>
		public Vector2 MeasureString(string text, int fontSize)
		{
			var font = FontSystem.GetFont(fontSize);
			return font.MeasureString(text);
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
				FontSystem.Dispose();
		}
	}
}
