#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec2 tex_coord;
layout(location = 0) out vec4 fsout_Color;

layout(set = 1, binding = 0) uniform texture2D Tex;
layout(set = 1, binding = 1) uniform sampler Sampler;

void main()
{
    fsout_Color = fsin_Color * texture(sampler2D(Tex, Sampler), tex_coord);
}