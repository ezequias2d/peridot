// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using Ez.Memory;
using System.Drawing;
using System.Numerics;

using Veldrid;

namespace Peridot
{
	/// <inheritdoc/>
	internal class VSpriteBatch : SpriteBatch<Texture>
	{
		private readonly GraphicsDevice m_device;
		private readonly DeviceBuffer m_vertexBuffer;
		private readonly Sampler m_sampler;
		private Pipeline m_pipeline;
		private readonly ResourceLayout[] m_resourceLayouts;
		private Dictionary<Texture, (DeviceBuffer buffer, ResourceSet set, ResourceSet textureSet)> m_buffers;

		internal VSpriteBatch(
			VPeridot peridot,
			Image whiteImage,
			OutputDescription outputDescription,
			Shader[] shaders,
			Sampler sampler,
			FaceCullMode cullMode,
			BlendStateDescription blendState,
			DepthStencilStateDescription depthStencil) : base(whiteImage)
		{
			m_device = peridot.GraphicsDevice;
			m_sampler = sampler ?? m_device.LinearSampler;
			m_vertexBuffer = CreateVertexBuffer(m_device);
			m_resourceLayouts = CreateResourceLayouts(m_device);
			m_buffers = new();

			var bs = blendState;
			var ds = depthStencil;
			m_pipeline = CreatePipeline(m_device, outputDescription, cullMode, bs, ds, shaders, m_resourceLayouts);
		}

		/// <summary>
		/// Draw this branch into a <see cref="CommandList"/>.
		/// Call this after calling <see cref="SpriteBatch{TTexture}.End"/>
		/// </summary>
		/// <param name="commandList"></param>
		public void DrawBatch(CommandList commandList)
		{
			var matrixSize = (int)MemUtil.SizeOf<Matrix4x4>();
			commandList.SetPipeline(m_pipeline);
			foreach (var item in m_batcher)
			{
				var texture = item.Key;
				var group = item.Value;

				var pair = GetBuffer(texture, group.Count + matrixSize);
				var mapped = m_device.Map(pair.buffer, MapMode.Write);

				MemUtil.Set(mapped.Data, ViewMatrix, 1);
				MemUtil.Copy(mapped.Data + matrixSize, group.GetSpan());

				m_device.Unmap(pair.buffer);

				commandList.SetVertexBuffer(0, m_vertexBuffer);
				commandList.SetGraphicsResourceSet(0, pair.set);
				commandList.SetGraphicsResourceSet(1, pair.textureSet);
				commandList.Draw(4, (uint)group.Count, 0, 0);
			}
		}

		internal (DeviceBuffer buffer, ResourceSet set, ResourceSet textureSet) GetBuffer(Texture t, int count)
		{
			var texture = t as Texture;
			var structSize = MemUtil.SizeOf<BatchItem>();
			var size = ((count + 63) & (~63)) * structSize;
			var bci = new BufferDescription((uint)size, BufferUsage.StructuredBufferReadOnly | BufferUsage.Dynamic, structSize);

			if (!m_buffers.TryGetValue(texture, out var pair))
			{
				pair.buffer = m_device.ResourceFactory.CreateBuffer(bci);
				pair.set = m_device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(m_resourceLayouts[0], pair.buffer));
				pair.textureSet = m_device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(m_resourceLayouts[1], texture, m_sampler));
				m_buffers[texture] = pair;
			}
			else if (size > pair.buffer.SizeInBytes)
			{
				pair.set.Dispose();
				pair.buffer.Dispose();

				pair.buffer = m_device.ResourceFactory.CreateBuffer(bci);
				pair.set = m_device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(m_resourceLayouts[0], pair.buffer));
				pair.textureSet = m_device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(m_resourceLayouts[1], texture, m_sampler));
				m_buffers[texture] = pair;
			}

			return pair;
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				m_vertexBuffer.Dispose();
			}
		}

		private static DeviceBuffer CreateVertexBuffer(GraphicsDevice device)
		{
			var bd = new BufferDescription()
			{
				SizeInBytes = 4 * MemUtil.SizeOf<Vector2>(),
				Usage = BufferUsage.VertexBuffer,
			};

			var buffer = device.ResourceFactory.CreateBuffer(bd);

			var vertices = new Vector2[]
			{
				new( 0,  0),
				new( 1,  0),
				new( 0,  1),
				new( 1,  1)
			};

			device.UpdateBuffer(buffer, 0, vertices);

			return buffer;
		}

		private static ResourceLayout[] CreateResourceLayouts(GraphicsDevice device)
		{
			var layouts = new ResourceLayout[2];

			var elements = new ResourceLayoutElementDescription[]
			{
				new("Items", ResourceKind.StructuredBufferReadOnly, ShaderStages.Vertex),
			};
			var rld = new ResourceLayoutDescription(elements);

			layouts[0] = device.ResourceFactory.CreateResourceLayout(rld);

			elements = new ResourceLayoutElementDescription[]
			{
				new("Tex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
			};
			rld = new ResourceLayoutDescription(elements);

			layouts[1] = device.ResourceFactory.CreateResourceLayout(rld);
			return layouts;
		}

		private static Pipeline CreatePipeline(GraphicsDevice device,
			OutputDescription outputDescription,
			FaceCullMode cullMode,
			BlendStateDescription blendState,
			DepthStencilStateDescription depthStencil,
			Shader[] shaders,
			params ResourceLayout[] layouts)
		{
			var vertexLayout = new VertexLayoutDescription(
				new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));

			var pipelineDescription = new GraphicsPipelineDescription
			{
				BlendState = blendState,
				DepthStencilState = depthStencil,
				RasterizerState = new(
					cullMode: cullMode,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology = PrimitiveTopology.TriangleStrip,
				ResourceLayouts = layouts,
				ShaderSet = new(
					vertexLayouts: new[] { vertexLayout },
					shaders: shaders,
					specializations: new[] { new SpecializationConstant(0, device.IsClipSpaceYInverted) }),
				Outputs = outputDescription,
			};

			return device.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
		}

		protected override Size GetSize(Texture image)
		{
			return new((int)image.Width, (int)image.Height);
		}
		
		protected override Texture GetHandle(Image image)
		{
			if (image is not VImage vimage)
				throw new InvalidCastException($"The {image} is not supported by this implementation.");
			return vimage.Texture;
		}
	}
}
