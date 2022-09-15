using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veldrid;

namespace Peridot
{
	/// <summary>
	/// Describes a sprite batch.
	/// </summary>
	public struct SpriteBatchDescriptor
	{
		/// <summary>
		/// Creates a sprite batch descriptior.
		/// </summary>
		/// <param name="outputDescription">The output description of target framebuffer.</param>
        /// <param name="shaders">The shaders to use to render. Uses <seealso cref="VPeridot.LoadDefaultShaders(GraphicsDevice)"/> for default.</param>
        /// <param name="sampler">The samppler used to sample.</param>
        /// <param name="cullMode">Controls which face will be culled. By default the sprite are rendered with forward normal, negatives scales can flips that normal.</param>
        /// <param name="blendState">The blend state description for creating the pipeline.</param>
        /// <param name="depthStencil">The depth stencil state description for creating the pipeline.</param>
		public SpriteBatchDescriptor(
			OutputDescription outputDescription,
			Shader[] shaders,
			Sampler? sampler = null,
			FaceCullMode cullMode = FaceCullMode.None,
			BlendStateDescription? blendState = null,
			DepthStencilStateDescription? depthStencil = null)
		{
			OutputDescription = outputDescription;
			Shaders = shaders;
			Sampler = sampler;
			CullMode = cullMode;
			BlendState = blendState;
			DepthStencil = depthStencil;
		}
		
		/// <summary>
		/// The output description of target framebuffer.
		/// </summary>
		public OutputDescription OutputDescription { init; get; }
		
		/// <summary>
		/// The shaders to use to render.
		/// Uses <seealso cref="VPeridot.LoadDefaultShaders(GraphicsDevice)"/> for default.
		/// </summary>
		public Shader[] Shaders { init; get; }
		
		/// <summary>
		/// The samppler used to sample.
		/// If null, will use LinearSampler.
		/// </summary>
		public Sampler? Sampler { init; get; }
		
		/// <summary>
		/// Controls which face will be culled.
		/// By default the sprite are rendered with forward normal, negatives scales can 
		/// flips that normal.
		/// </summary>
		public FaceCullMode CullMode { init; get; }
		
		/// <summary>
		/// The blend state description for creating the pipeline.
		/// If null, will use SingleAlphaBlend.
		/// </summary>
		public BlendStateDescription? BlendState { init; get; }
		
		/// <summary>
		/// The depth stencil state description for creating the pipeline.
		/// If null, will use depth test, depth write enable and comparison kind GreaterEqual.
		/// </summary>
		public DepthStencilStateDescription? DepthStencil { init; get; }
	}
}
