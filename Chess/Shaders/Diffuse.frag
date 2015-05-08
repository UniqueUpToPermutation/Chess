#version 330 core

in vec2 UV;

uniform sampler2D Diffuse;

out vec3 Color;

void main()
{
	Color = texture(Diffuse, UV).xyz;
}