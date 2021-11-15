// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Drawing;

namespace Peridot
{
    /// <summary>
    /// Represents a 2D texture to use in <see cref="SpriteBatch{TTexture}"/>.
    /// </summary>
    public interface ITexture2D : IDisposable
    {
        /// <summary>
        /// A bool indicating whether this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; }

        /// <summary>
        /// The total size of the texture.
        /// </summary>
        public Size Size { get; }
    }
}
