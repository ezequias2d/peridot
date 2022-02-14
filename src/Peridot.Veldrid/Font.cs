// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using FontStashSharp;
using System.Numerics;

namespace Peridot.Veldrid
{
    /// <summary>
    /// Represents a font that can be rendered with <see cref="TextRenderer"/>.
    /// </summary>
    public class Font : IFont
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
        /// Destroys a instance of <see cref="TextureWrapper"/>.
        /// </summary>
        ~Font()
        {
            Dispose(false);
        }

        internal FontSystem FontSystem { get; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
