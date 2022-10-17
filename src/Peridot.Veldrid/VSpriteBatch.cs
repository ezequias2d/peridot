using System.Collections.Generic;
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
        private readonly SortMode m_sortMode;
        private Pipeline m_pipeline;
        private readonly ResourceLayout[] m_resourceLayouts;

        private readonly DeviceBuffer m_viewBuffer;
        private readonly ResourceSet m_viewSet;
        private DeviceBuffer m_buffer;
        private ResourceSet m_bufferSet;

        private Dictionary<Texture, ResourceSet> m_textureSets;

        internal VSpriteBatch(
            VPeridot peridot,
            Image whiteImage,
            OutputDescription outputDescription,
            Shader[] shaders,
            Sampler sampler,
            FaceCullMode cullMode,
            BlendStateDescription blendState,
            DepthStencilStateDescription depthStencil,
            SortMode sortMode) : base(whiteImage)
        {
            m_device = peridot.GraphicsDevice;
            m_sampler = sampler ?? m_device.LinearSampler;
            m_vertexBuffer = CreateVertexBuffer(m_device);
            m_resourceLayouts = CreateResourceLayouts(m_device);
            m_textureSets = new();

            var bs = blendState;
            var ds = depthStencil;
            m_pipeline = CreatePipeline(m_device, outputDescription, cullMode, bs, ds, shaders, m_resourceLayouts);
            m_sortMode = sortMode;

            m_viewBuffer = m_device.ResourceFactory.CreateBuffer(new BufferDescription(MemUtil.SizeOf<Matrix4x4>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            m_viewSet = m_device.ResourceFactory.CreateResourceSet(new(m_resourceLayouts[2], m_viewBuffer));
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

            m_batcher.Build(m_sortMode);
            var (buffer, bufferSet) = GetBuffer(m_batcher.Items.Length);
            var itemSize = MemUtil.SizeOf<BatchItem>();

            var mapped = m_device.Map(m_viewBuffer, MapMode.Write);
            MemUtil.Set(mapped.Data, ViewMatrix, 1);
            m_device.Unmap(m_viewBuffer);

            mapped = m_device.Map(buffer, MapMode.Write);
            MemUtil.Copy(mapped.Data, m_batcher.Items);
            m_device.Unmap(buffer);

            foreach (var slice in m_batcher.Slices)
            {
                var textureSet = GetTextureSet(slice.Texture);

                commandList.SetVertexBuffer(0, m_vertexBuffer);
                uint offset = (uint)slice.Start * itemSize;
                commandList.SetGraphicsResourceSet(0, bufferSet, 1, ref offset);
                commandList.SetGraphicsResourceSet(1, textureSet);
                commandList.SetGraphicsResourceSet(2, m_viewSet);
                commandList.Draw(4, (uint)slice.Length, 0, 0);
            }
        }

        internal (DeviceBuffer, ResourceSet) GetBuffer(int count)
        {
            var structSize = MemUtil.SizeOf<BatchItem>();
            var size = ((count + 63) & (~63)) * structSize;
            var bci = new BufferDescription((uint)size, BufferUsage.StructuredBufferReadOnly | BufferUsage.Dynamic, structSize);

            if (m_buffer == null || m_buffer.SizeInBytes < size)
            {
                if (m_buffer != null)
                {
                    m_buffer.Dispose();
                    m_bufferSet.Dispose();
                }
                m_buffer = m_device.ResourceFactory.CreateBuffer(bci);
                m_bufferSet = m_device.ResourceFactory.CreateResourceSet(new(m_resourceLayouts[0], m_buffer));
                m_buffer.Name = "ItemBuffer";
            }

            return (m_buffer, m_bufferSet);
        }

        internal uint CalculateStructuredAlignment<T>() where T : unmanaged
        {
            var minUbo = m_device.StructuredBufferMinOffsetAlignment;

            var alignment = MemUtil.SizeOf<T>();
            if (minUbo > 0)
            {
                alignment = (alignment + minUbo - 1) & ~(minUbo - 1);
            }
            return alignment;
        }

        internal ResourceSet GetTextureSet(Texture t)
        {
            var texture = t as Texture;
            var structSize = MemUtil.SizeOf<BatchItem>();

            if (!m_textureSets.TryGetValue(texture, out var set))
            {
                set =
                m_textureSets[texture] =
                    m_device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(m_resourceLayouts[1], texture, m_sampler));
            }
            return set;
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
            var layouts = new ResourceLayout[3];

            var elements = new ResourceLayoutElementDescription[]
            {
                new("Items", ResourceKind.StructuredBufferReadOnly, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding),
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

            elements = new ResourceLayoutElementDescription[]
            {
                new("View", ResourceKind.UniformBuffer, ShaderStages.Vertex)
            };
            rld = new ResourceLayoutDescription(elements);
            layouts[2] = device.ResourceFactory.CreateResourceLayout(rld);

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
