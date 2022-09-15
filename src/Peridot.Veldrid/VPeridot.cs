using System.Diagnostics;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using VPF = Veldrid.PixelFormat;
namespace Peridot;

public class VPeridot : IPeridot<Texture, SpriteBatchDescriptor>
{
	private readonly GraphicsDevice m_gd;
	private readonly Image m_whiteImage;
	public VPeridot(GraphicsDevice gd)
	{
		m_gd = gd;
		m_whiteImage = CreateImage(new ImageDescription()
		{
			Format = PixelFormat.BGRA8,
			Width = 1,
			Height = 1,
		});

		UpdateImage<byte>(m_whiteImage, new byte[] { 255, 255, 255, 255 }, 0, 0, 1, 1);
	}

	internal GraphicsDevice GraphicsDevice => m_gd;

	/// <inheritdoc/>
	public Image CreateImage(ImageDescription description)
	{
		var pf = description.Format switch
		{
			PixelFormat.R8 => VPF.R8_UNorm,
			PixelFormat.R16 => VPF.R16_UNorm,
			PixelFormat.BGRA8 => VPF.B8_G8_R8_A8_UNorm,
			PixelFormat.RGBAF32 => VPF.R32_G32_B32_A32_Float,
			PixelFormat.RG8 => VPF.R8_G8_UNorm,
			PixelFormat.RG16 => VPF.R16_G16_UNorm,
			PixelFormat.RGBAI32 => VPF.R8_G8_B8_A8_SInt,
			PixelFormat.RGBA8 => VPF.R8_G8_B8_A8_UNorm,
			_ => throw new ArgumentOutOfRangeException(nameof(description.Format))
		};
		TextureDescription desc = new(description.Width, description.Height, 1, 1, 1, pf, TextureUsage.Sampled, TextureType.Texture2D);
		return new VImage(m_gd.ResourceFactory.CreateTexture(desc));
	}

	/// <inheritdoc/>
	public void UpdateImage<T>(Image image, ReadOnlySpan<T> source, uint x, uint y, uint width, uint height) where T : unmanaged
	{
		Debug.Assert(image is VImage);
		if (image is not VImage vimage)
			throw new ArgumentOutOfRangeException(nameof(image));

		unsafe
		{
			uint size = (uint)source.Length * (uint)sizeof(T);
			fixed (T* ptr = source)
			{
				m_gd.UpdateTexture(vimage, (IntPtr)ptr, size, x, y, 0, width, height, 1, 0, 0);
			}
		}
	}

	/// <inheritdoc/>
	public ISpriteBatch<Texture> CreateSpriteBatch(SpriteBatchDescriptor descriptor)
	{
		var s = descriptor.Sampler ?? GraphicsDevice.LinearSampler;
		var bs = descriptor.BlendState ?? BlendStateDescription.SingleAlphaBlend;
		var ds = descriptor.DepthStencil ?? new(
			depthTestEnabled: true,
			depthWriteEnabled: true,
			comparisonKind: ComparisonKind.GreaterEqual);

		return new VSpriteBatch(
			this,
			m_whiteImage,
			descriptor.OutputDescription,
			descriptor.Shaders,
			s,
			descriptor.CullMode,
			bs,
			ds);
	}

	/// <summary>
	/// Loads default shaders for <see cref="ISpriteBatch"/>.
	/// </summary>
	/// <returns>Shaders for use in <see cref="ISpriteBatch"/>.</returns>
	public Shader[] LoadDefaultShaders()
	{

		var vertexShaderDesc = new ShaderDescription(
			ShaderStages.Vertex,
			Encoding.UTF8.GetBytes(Resource.shader_vert),
			"main");

		var fragmentShaderDesc = new ShaderDescription(
			ShaderStages.Fragment,
			Encoding.UTF8.GetBytes(Resource.shader_frag),
			"main");

		return GraphicsDevice.ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
	}

	ISpriteBatch IPeridot<SpriteBatchDescriptor>.CreateSpriteBatch(SpriteBatchDescriptor description)
	{
		return CreateSpriteBatch(description);
	}
}
