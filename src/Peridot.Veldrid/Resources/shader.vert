#version 450

layout(constant_id = 0) const bool InvertY = false;

layout(location = 0) in vec2 Position;
layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec2 tex_coord;
layout(location = 2) out vec4 bounds;
layout(location = 3) out vec2 pos;

struct Item 
{
    vec4 uv;
    vec4 color;
    vec2 scale;
    vec2 origin;
    vec4 location;
    vec4 scissor;
};

layout(std430, set = 0, binding = 0) readonly buffer Items
{
    Item items[];
};

layout(std140, set = 2, binding = 0) uniform View 
{
    mat4 view;
};

mat2 makeRotation(float angle)
{
    float c = cos(angle);
    float s = sin(angle);
    return mat2(c, -s, s, c);
}

void main() {
    Item item = items[gl_InstanceIndex];

    float angle = item.location.w;
    pos = Position * item.scale;
    pos -= item.origin;
    pos *= makeRotation(item.location.w);
    pos += item.location.xy;

    tex_coord = Position * item.uv.zw + item.uv.xy;

    // scissor bounds
    vec2 start = item.scissor.xy;
    vec2 end = start + item.scissor.zw;
    bounds = vec4(start, end);

    gl_Position = view * vec4(pos, item.location.z, 1);
    pos = gl_Position.xy;

    if (!InvertY)
        gl_Position.y = -gl_Position.y;

    fsin_Color = item.color;
}