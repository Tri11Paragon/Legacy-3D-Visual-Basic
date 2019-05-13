#version 330 core

layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Color;

out VS_OUTPUT {
    vec3 Color;
} OUT;

uniform float time;

void main()
{
    gl_Position = vec4(vec3(Position.x + time, Position.y + time, Position.z), 1.0);
    OUT.Color = Color;
}