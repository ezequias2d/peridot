#version 450

layout (constant_id = 0) const bool InvertY = false;

layout(location = 0) in vec2 Position;
layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec2 tex_coord;

struct Item 
{
    mat4 source;
    vec4 color;
    mat4 model;
    mat4 projection;
};

layout(std430, binding = 0) readonly buffer Items
{
    mat4 view;
    Item items[];
};

void main()
{
    Item item = items[gl_InstanceIndex];
    tex_coord = (item.source * vec4(Position, 0, 1)).xy;

    gl_Position = item.projection * view * item.model * vec4(Position, 0, 1);

    if(InvertY)
        gl_Position.y = -gl_Position.y;

    fsin_Color = item.color;
}