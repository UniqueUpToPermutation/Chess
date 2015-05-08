#version 330 core

uniform vec3 Color;
uniform vec3 LightDirection = vec3(0, -1, 1);

out vec3 OutputColor;

in vec4 Normal;

void main()
{
	OutputColor = Color * (-1f * dot(vec4(LightDirection, 0), normalize(Normal)));
}