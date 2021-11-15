// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Ez.Memory;

using System.Numerics;
using System.Text;

using Veldrid;
using Veldrid.SPIRV;

namespace Peridot.Veldrid
{
    /// <inheritdoc/>
    public class VeldridSpriteBatch : SpriteBatch<TextureWrapper>
    {
        private readonly GraphicsDevice _device;
        private readonly DeviceBuffer _vertexBuffer;
        private readonly Sampler _sampler;
        private Pipeline _pipeline;
        private readonly ResourceLayout[] _resourceLayouts;
        private Dictionary<Texture, (DeviceBuffer buffer, ResourceSet set, ResourceSet textureSet)> _buffers;

        /// <summary>
        /// Creates a new instance of <see cref="VeldridSpriteBatch"/>.
        /// </summary>
        /// <param name="device">The graphics device to create resources.</param>
        /// <param name="outputDescription">The output description of target framebuffer.</param>
        /// <param name="shaders">The shaders to use to render. Uses <seealso cref="LoadDefaultShaders(GraphicsDevice)"/> for default.</param>
        /// <param name="sampler">The samppler used to sample.</param>
        public VeldridSpriteBatch(GraphicsDevice device,
            OutputDescription outputDescription,
            Shader[] shaders,
            Sampler? sampler = null,
            BlendStateDescription? blendState = null,
            DepthStencilStateDescription? depthStencil = null) : base()
        {
            _sampler = sampler ?? device.LinearSampler;
            _device = device;
            _vertexBuffer = CreateVertexBuffer(device);
            _resourceLayouts = CreateResourceLayouts(device);
            _buffers = new();

            var bs = blendState ?? BlendStateDescription.SingleAlphaBlend;
            var ds = depthStencil ?? new(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);
            _pipeline = CreatePipeline(device, outputDescription, bs, ds, shaders, _resourceLayouts);
        }

        /// <summary>
        /// Draw this branch into a <see cref="CommandList"/>.
        /// Call this after calling <see cref="SpriteBatch.End"/>
        /// </summary>
        /// <param name="commandList"></param>
        public void DrawBatch(CommandList commandList)
        {
            var matrixSize = (int)MemUtil.SizeOf<Matrix4x4>();
            commandList.SetPipeline(_pipeline);
            foreach (var item in _batcher)
            {
                var texture = item.Key;
                var group = item.Value;

                var pair = GetBuffer(texture, group.Count + matrixSize);
                var mapped = _device.Map(pair.buffer, MapMode.Write);

                MemUtil.Set(mapped.Data, ViewMatrix, 1);
                MemUtil.Copy(mapped.Data + matrixSize, group.GetSpan());

                _device.Unmap(pair.buffer);

                commandList.SetVertexBuffer(0, _vertexBuffer);
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

            if (!_buffers.TryGetValue(texture, out var pair))
            {
                pair.buffer = _device.ResourceFactory.CreateBuffer(bci);
                pair.set = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[0], pair.buffer));
                pair.textureSet = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[1], texture, _sampler));
                _buffers[texture] = pair;
            }
            else if (size > pair.buffer.SizeInBytes)
            {
                pair.set.Dispose();
                pair.buffer.Dispose();

                pair.buffer = _device.ResourceFactory.CreateBuffer(bci);
                pair.set = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[0], pair.buffer));
                pair.textureSet = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[1], texture, _sampler));
                _buffers[texture] = pair;
            }

            return pair;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _vertexBuffer.Dispose();
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
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: false,
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

        /// <summary>
        /// Loads default shaders for <see cref="VeldridSpriteBatch"/>.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <returns>Shaders for use in <see cref="VeldridSpriteBatch"/>.</returns>
        public static Shader[] LoadDefaultShaders(GraphicsDevice graphicsDevice)
        {

            var vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(Resource.shader_vert),
                "main");

            var fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(Resource.shader_frag),
                "main");

            return graphicsDevice.ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
        }
    }
}
