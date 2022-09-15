// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Veldrid;

namespace Peridot;

/// <summary>
/// A wrapper with <see cref="VImage"/> interface.
/// </summary>
internal class VImage : Image, IEquatable<VImage>
{
	/// <summary>
	/// Creates a new instance of <see cref="VImage"/>.
	/// </summary>
	/// <param name="texture">The texture.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public VImage(Texture texture)
	{
		Texture = texture ?? throw new ArgumentNullException(nameof(texture));
	}

	/// <summary>
	/// Texture
	/// </summary>
	public Texture Texture { get; }

	/// <summary>
	/// Size of texture
	/// </summary>
	public Size Size => new((int)Texture.Width, (int)Texture.Height);

	/// <inheritdoc/>
	public bool IsDisposed => Texture.IsDisposed;

	/// <inheritdoc/>
	public PixelFormat Format => Texture.Format switch 
	{
		Veldrid.PixelFormat.R8_UNorm => PixelFormat.R8,
		Veldrid.PixelFormat.R16_UNorm => PixelFormat.R16,
		Veldrid.PixelFormat.B8_G8_R8_A8_UNorm => PixelFormat.BGRA8,
		Veldrid.PixelFormat.R32_G32_B32_A32_Float => PixelFormat.RGBAF32,
		Veldrid.PixelFormat.R8_G8_UNorm => PixelFormat.RG8,
		Veldrid.PixelFormat.R16_G16_UNorm => PixelFormat.RG16,
		Veldrid.PixelFormat.R32_G32_B32_A32_SInt => PixelFormat.RGBAI32,
		Veldrid.PixelFormat.R8_G8_B8_A8_UNorm => PixelFormat.RGBA8,
		_ => PixelFormat.Undefined
	};

	/// <inheritdoc/>
	public override int GetHashCode() => Texture.GetHashCode();

	/// <inheritdoc/>
	public bool Equals(VImage? other) => Texture == other?.Texture;

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is VImage tw && Equals(tw);

	/// <inheritdoc/>
	public override string? ToString() => Texture.ToString();

	/// <inheritdoc/>
	public void Dispose()
	{
		Texture.Dispose();
		GC.SuppressFinalize(this);
	}

	public static implicit operator VImage(Texture t) => new(t);

	public static implicit operator Texture(VImage t) => t.Texture;
	
	public static bool operator ==(VImage left, VImage right) => left.Equals(right);

	public static bool operator !=(VImage left, VImage right) => !(left == right);
}
