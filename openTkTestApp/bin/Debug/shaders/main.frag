#version 330 core

in VS_OUTPUT {
    vec3 Color;
} IN;

out vec4 Color;

uniform float time;

void main()
{
    Color = vec4(vec3(IN.Color.x + time, IN.Color.y + time, IN.Color.z + time), 1.0f);
}