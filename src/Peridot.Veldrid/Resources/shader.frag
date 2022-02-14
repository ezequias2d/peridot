#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec2 tex_coord;
layout(location = 2) in vec4 scissor;
layout(location = 3) in vec2 pos;
layout(location = 0) out vec4 fsout_Color;

layout(set = 1, binding = 0) uniform texture2D Tex;
layout(set = 1, binding = 1) uniform sampler Sampler;

void main()
{
    float left = scissor.x;
    float top = scissor.y;
    float right = scissor.z;
    float bottom = scissor.w;

    if(!(left <= pos.x && right >= pos.x &&
        top <= pos.y && bottom >= pos.y))
        discard;

    fsout_Color = fsin_Color * texture(sampler2D(Tex, Sampler), tex_coord);
}